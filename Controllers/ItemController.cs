using FOODPEDI.API.REST.DataAccess;
using FOODPEDI.API.REST.DataAccess.Entities;
using FOODPEDI.API.REST.Helpers;
using FOODPEDI.API.REST.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FOODPEDI.API.REST.Controllers
{
    public class ItemController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public ItemController(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpGet("basic-list")]
        //[Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> BasicList(string categoryId ,int page, int pageCount=20)
        {
            try
            {

                using (AppDbContext dbContext = new AppDbContext())
                {
                    var data = await dbContext.Items
                        .Where(x => x.ItemCategories.FirstOrDefault(c=>c.CategoryId==categoryId).CategoryId==categoryId && x.State == ItemState.Confirmed)
                        .Skip((page-1)*pageCount).Take(pageCount)
                        .Include(c=>c.ItemCategories).ThenInclude(c=>c.Category).Include(i=>i.ItemImages)
                        .Select(x=> new { 
                            Id=x.Id,
                            Name=x.Name,
                            ShortDescription=x.ShortDescription1,
                            CategoryName=x.ItemCategories.FirstOrDefault().Category.Name,
                            CategoryId=x.ItemCategories.FirstOrDefault().CategoryId,
                            ItemWeight=123,
                            PostDate=x.CreateDate.ToString("g"),
                            DisplayCount=32,
                            LikeCount=12,
                            ImageUrl= $"{ _configuration["Domain:Development"]}item/file/{x.ImageFolderPath}/{x.ItemImages.FirstOrDefault().Id}"
                    }).ToListAsync();

                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("full-search")]
        //[Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> FullSearchList(string searchText)
        {
            try
            {

                using (AppDbContext dbContext = new AppDbContext())
                {
                    var tasks = new List<Task<List<MainSearchDto>>>();

                    Task<List<MainSearchDto>> itemsTask = dbContext.Items
                        .Where(x=>x.Name.ToUpper().Contains(searchText.ToUpper()))
                        .Select(x => new MainSearchDto
                         {
                            Id=x.Id,
                            Name = x.Name,
                            Group = "Item"    
                        }).ToListAsync();



                    Task<List<MainSearchDto>> categoriesTask = dbContext.Categories
                        .Where(x => x.Name.ToUpper().Contains(searchText.ToUpper()))
                        .Select(x => new MainSearchDto
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Group = "Category"
                        }).ToListAsync();

                    tasks.Add(itemsTask);
                    tasks.Add(categoriesTask);

                    var result = await Task.WhenAll(tasks);
                  
                    

                    return Ok(
                        new {
                            Items = tasks[0].Result,
                            Categories = tasks[1].Result
                        });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [AllowAnonymous]
        [HttpGet("my-items")]
        //[Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> BasicListForUser(ItemState? itemState)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                using (AppDbContext dbContext = new AppDbContext())
                {
                    var data = await dbContext.Items.Where(x=>x.CreateUser==userId && (itemState != null ? x.State == itemState:true)).Include(c => c.ItemCategories).ThenInclude(c => c.Category).Include(i => i.ItemImages).Select(x => new {
                        Id=x.Id,
                        Name = x.Name,
                        ShortDescription = x.ShortDescription1,
                        CategoryName = x.ItemCategories.FirstOrDefault().Category.Name,
                        CategoryId = x.ItemCategories.FirstOrDefault().CategoryId,
                        State= x.State,
                        ItemWeight = 123,
                        PostDate = x.CreateDate.ToString("g"),
                        DisplayCount = 32,
                        LikeCount = 12,
                        ImageUrl = $"{ _configuration["Domain:Development"]}item/file/{x.ImageFolderPath}/{x.ItemImages.FirstOrDefault().Id}"
                    }).ToListAsync();
                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("file/{folder1}/{folder2}/{folder3}/{folder4}/{id}")]
        public async Task<IActionResult> Image(string folder1, string folder2, string folder3, string folder4, string id)
        {
            try
            {
                var path = DirectoryHelper.GetLocalDataPath("files/") + $"{folder1}/{folder2}/{folder3}/{folder4}/" + $"{id}.jpeg";
                Byte[] byteArray = System.IO.File.ReadAllBytes(@$"{path}");   // You can use your own method over here.         
                return File(byteArray, "image/png");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [AllowAnonymous]
        [HttpPost("edit")]
        //[Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Edit(ItemEditModel editItemModel)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var currDate = DateTime.Now;


                using (AppDbContext dbContext = new AppDbContext())
                {
                    Brand brand = await dbContext.Brands.FirstOrDefaultAsync(x=>x.Id == editItemModel.Brand.Id);

                    if (brand == null)
                    {
                        brand = new Brand
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = editItemModel.Brand.Name
                        };

                        await dbContext.Brands.AddAsync(brand);
                    }

                    var oldItem = await dbContext.Items.AsNoTracking().FirstOrDefaultAsync(x => x.Id == editItemModel.Id);

                    if (oldItem!=null)
                    {

                        var deleteCategories = await dbContext.ItemCategories.Where(x => x.ItemId == editItemModel.Id && !editItemModel.Categories.Contains(x.CategoryId)).ToListAsync();

                        if(deleteCategories.Count()>0)
                            dbContext.ItemCategories.RemoveRange(deleteCategories);

                        var categories = new List<ItemCategory>();
                        if (editItemModel.Categories.Length >= 1)
                            foreach (var item in editItemModel.Categories)
                            {

                                categories.Add(new ItemCategory {
                                    Id = Guid.NewGuid().ToString(),
                                    CategoryId = item 
                                }) ;
                            }




                        //Add new Ingredient on db

                        var ingredientOnDb = await dbContext.Ingredients.Where(x => editItemModel.Ingredients.Select(s => s.IngredientId).Contains(x.Id)).ToListAsync();
                        
                        if (editItemModel.Ingredients.Any(x =>!ingredientOnDb.Select(s=>s.Id).Contains(x.IngredientId)))
                        {
                            foreach (var item in editItemModel.Ingredients.Where(x => !ingredientOnDb.Select(s => s.Id).Contains(x.IngredientId)))
                            {
                                item.IngredientId = Guid.NewGuid().ToString();
                            }

                            var newIngredients = editItemModel.Ingredients.Where(x => !ingredientOnDb.Select(s => s.Id).Contains(x.IngredientId)).Select(x => new Ingredient
                            {
                                Id=x.IngredientId,
                                Name = x.IngredientName
                            }).ToList();

                            await dbContext.Ingredients.AddRangeAsync(newIngredients);
                        }

                        //Delete old ingredients
                        var deleteIngredients = await dbContext.ItemIngredients.Where(x => x.ItemId == editItemModel.Id).ToListAsync();
                        dbContext.ItemIngredients.RemoveRange(deleteIngredients);

                        //Add old ingredients
                        var ingredients = editItemModel.Ingredients.Select(x=> new ItemIngredient { 
                            Id=Guid.NewGuid().ToString(),
                            IngredientId=x.IngredientId,
                            Value=x.Value,
                            UnitCode=x.UnitCode
                        }).ToList();

                        await dbContext.AddRangeAsync(ingredients);

                    
                        var random = new Random();
                        var images = new List<ItemImage>();
                        var mainFolder = random.Next(100, 199);
                        var subFolder = random.Next(100, 199);

                        var oldImages = await dbContext.ItemImages.AsNoTracking().Where(x => x.ItemId == editItemModel.Id).ToListAsync();
                        var deleteImages = oldImages.Where(x => !editItemModel.Images.Select(s => s.Id).Contains(x.Id));
                        dbContext.ItemImages.RemoveRange(deleteImages);


                        var updateImages = new List<ItemImage>();
                        foreach (var item in editItemModel.Images.Where(i => oldImages.Where(o => !deleteImages.Select(d => d.Id).Contains(o.Id)).Select(s => s.Id).Contains(i.Id)))
                        {
                            updateImages.Add(new ItemImage
                            {
                                Id = item.Id,
                                ItemId = editItemModel.Id,
                                IsMainImage = item.Selected,
                                OrderNumber = (short)item.Order
                            });
                        }

                        dbContext.ItemImages.UpdateRange(updateImages);

                        var newImages = new List<ItemImage>();

                        foreach (var item in editItemModel.Images.Where(i=>!oldImages.Select(s=>s.Id).Contains(i.Id)))
                        {
                            newImages.Add(new ItemImage
                            {
                                Id = item.Id,
                                ItemId=editItemModel.Id,
                                IsMainImage = item.Selected,
                                OrderNumber = (short)item.Order
                            });
                        }

                        await dbContext.ItemImages.AddRangeAsync(newImages);

                        



                        var entity = new Item
                        {
                            Id = editItemModel.Id,
                            Barcode1 = editItemModel.Barcode1,
                            Description1 = editItemModel.Description1,
                            MadeCityId = editItemModel.MadeCityId,
                            MadeCountryId = editItemModel.MadeCountryId,
                            MadeStateId = editItemModel.MadeStateId,
                            WebPageUrl1 = editItemModel.WebPageUrl1,
                            Name = editItemModel.Name,
                            ShortDescription1 = editItemModel.ShortDescription1,
                            ItemCategories = categories,
                            ImageFolderPath=oldItem.ImageFolderPath,
                            ItemIngredients = ingredients,
                            ItemWeight = editItemModel.ItemWeight,
                            BrandId = brand.Id,
                            CreateUser = oldItem.CreateUser,
                            CreateDate = oldItem.CreateDate,
                            UpdateDate = currDate,
                            UpdateUser = userId
                        };


                        var updatedEntity = dbContext.Entry(entity);
                        updatedEntity.State = EntityState.Modified;


                        await dbContext.SaveChangesAsync();

                        foreach (var item in editItemModel.Images.Where(x => deleteImages.Select(s => s.Id).Contains(x.Id)))
                        {
                            System.IO.File.Delete(DirectoryHelper.GetLocalDataPath($"files/{oldItem.ImageFolderPath}") + $"/{item.Id}.jpeg");
                        }

                        foreach (var item in editItemModel.Images.Where(x=>newImages.Select(s=>s.Id).Contains(x.Id)))
                        {
                            System.IO.File.WriteAllBytes(DirectoryHelper.GetLocalDataPath($"files/{oldItem.ImageFolderPath}") + $"/{item.Id}.jpeg",
                               Convert.FromBase64String(item.File.Substring(item.File.IndexOf(",") + 1)));
                        }

                        return Created("", entity);
                    }
                    else
                    {
                        var categories = new List<ItemCategory>();
                        if (editItemModel.Categories.Length >= 1)
                            foreach (var item in editItemModel.Categories)
                            {
                                categories.Add(new ItemCategory { 
                                    Id = Guid.NewGuid().ToString(),
                                    CategoryId = item 
                                });
                            }

                        //Add new Ingredient on db
                        if (editItemModel.Ingredients.Count()>0)
                        {
                            var insertedData = await dbContext.Ingredients.Where(x => editItemModel.Ingredients.Select(s => s.IngredientId).Contains(x.Id)).ToListAsync();

                            foreach (var item in editItemModel.Ingredients.Where(x => !insertedData.Select(s => s.Id).Contains(x.IngredientId)))
                            {
                                item.IngredientId = Guid.NewGuid().ToString();
                            }

                            var newIngredients = editItemModel.Ingredients.Where(x => !insertedData.Select(s => s.Id).Contains(x.IngredientId)).Select(x => new Ingredient
                            {
                                Id = x.IngredientId,
                                Name = x.IngredientName
                            }).ToList();

                            await dbContext.Ingredients.AddRangeAsync(newIngredients);
                        }



                        var ingredients = editItemModel.Ingredients.Select(item=> new ItemIngredient
                        {
                            Id=Guid.NewGuid().ToString(),
                            IngredientId = item.IngredientId,
                            Value = item.Value,
                            UnitCode = item.UnitCode
                        }).ToList();

                        
                        var images = new List<ItemImage>();

                        var random = new Random();
                        var mainFolder = random.Next(100, 199);
                        var subFolder = random.Next(100, 199);
                        var imageFolder = $"item/image/{mainFolder}/{subFolder}";

                        foreach (var item in editItemModel.Images)
                        {
                            images.Add(new ItemImage
                            {
                                Id = item.Id,
                                IsMainImage = item.Selected,
                                OrderNumber = (short)item.Order
                            });
                        }
     
                        var entity = new Item
                        {
                            Id = Guid.NewGuid().ToString(),
                            Barcode1 = editItemModel.Barcode1,
                            Description1 = editItemModel.Description1,
                            MadeCityId = editItemModel.MadeCityId,
                            MadeCountryId = editItemModel.MadeCountryId,
                            MadeStateId = editItemModel.MadeStateId,
                            WebPageUrl1 = editItemModel.WebPageUrl1,
                            Name = editItemModel.Name,
                            ShortDescription1 = editItemModel.ShortDescription1,
                            ItemCategories = categories,
                            ItemIngredients = ingredients,
                            ItemWeight = editItemModel.ItemWeight,
                            ItemImages = images,
                            ImageFolderPath = imageFolder,
                            BrandId = brand.Id,
                            CreateUser = userId,
                            CreateDate = currDate,
                            UpdateDate = currDate,
                            UpdateUser = userId
                        };

                        await dbContext.Items.AddAsync(entity);

                        await dbContext.SaveChangesAsync();
                   
                        foreach (var item in editItemModel.Images)
                            System.IO.File.WriteAllBytes(DirectoryHelper.GetLocalDataPath($"files/{imageFolder}") + $"/{item.Id}.jpeg", 
                                Convert.FromBase64String(item.File.Substring(item.File.IndexOf(",") + 1)));

                        return Created("", entity);
                    }

                    
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        [AllowAnonymous]
        [HttpGet("get-for-edit")]
        //[Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetForEdit(string Id)
        {
            try
            {
                using (AppDbContext dbContext = new AppDbContext())
                {
                    


                    var result = await dbContext.Items.Where(x=>x.Id==Id)
                        .Select(x=>new { 
                            Id = x.Id,
                            Barcode1 = x.Barcode1,
                            Description1 = x.Description1,
                            MadeCityId = x.MadeCityId,
                            MadeCityName = x.MadeCity.Name,
                            MadeCountryId = x.MadeCountryId,
                            MadeCountryName=x.MadeCountry.Name,
                            MadeStateId = x.MadeStateId,
                            MadeStateName=x.MadeState.Name,
                            WebPageUrl1 = x.WebPageUrl1,
                            Name = x.Name,
                            ShortDescription1 = x.ShortDescription1,
                            ItemWeight = x.ItemWeight,
                            BrandId = x.BrandId,
                            BrandName=x.Brand.Name,
                            CreateDate = x.CreateDate,
                            UpdateDate = x.UpdateDate,
                            
                            Categories = x.ItemCategories.Select(c=>new { c.CategoryId, CategoryName = c.Category.Name }),
                            Ingredients = x.ItemIngredients.Select(c => new { c.IngredientId, c.Ingredient, c.Value,c.UnitCode }),
                            Images = x.ItemImages.Select(i=>new { i.Id, Order= i.OrderNumber, Selected = i.IsMainImage ,ImageUrl = $"{ _configuration["Domain:Development"]}item/file/{x.ImageFolderPath}/{i.Id}"}).OrderBy(o=>o.Order).ToList()
                        })
                        .ToListAsync();                    

                    return Ok( result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       
    }
}
