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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FOODPEDI.API.REST.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public CategoryController(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        
        [HttpPost("Add")]
        //[Authorize(Roles="User,Admin")]
        public async Task<IActionResult> Add(CategoryEditModel categoryEditModel)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = new AppUser { Id = userId };
                var currDate = DateTime.Now;

                var random = new Random();
                var mainFolder = random.Next(10, 99);
                var subFolder = random.Next(10, 99);
                var imageFolder = $"category/image/{mainFolder}/{subFolder}";
                var iconFolder = $"category/icon/{mainFolder}/{subFolder}";

                using (AppDbContext dbContext = new AppDbContext())
                {
                    var oldCategory = await dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == categoryEditModel.Id);
                    var parentCategory = await dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == categoryEditModel.ParentId);
                    
                    
                    

                    if (oldCategory != null)
                    {
                        var entity = new Category
                        {
                            Id = oldCategory.Id,
                            ParentId = categoryEditModel.HasChild ? null : (parentCategory==null?null: parentCategory.Id),
                            IsActive = categoryEditModel.IsActive,
                            HasChild = categoryEditModel.HasChild,
                            Name = categoryEditModel.Name,
                            Description1 = categoryEditModel.Description1,
                            Description2 = categoryEditModel.Description2,
                            Description3 = categoryEditModel.Description3,
                            ImageFolderPath = oldCategory.ImageFolderPath == null? imageFolder:oldCategory.ImageFolderPath,
                            IconFolderPath = oldCategory.IconFolderPath == null ? iconFolder : oldCategory.IconFolderPath,
                            CreateUser = oldCategory.CreateUser,
                            CreateDate = oldCategory.CreateDate,
                            UpdateDate = currDate,
                            UpdateUser = userId
                        };

                        if (categoryEditModel.Image1 != null)
                            entity.ImageId1 = oldCategory.ImageId1 == categoryEditModel.Image1.Id ? oldCategory.ImageId1 : categoryEditModel.Image1.Id;

                        if (categoryEditModel.Image2 != null)
                            entity.ImageId2 = oldCategory.ImageId2 == categoryEditModel.Image2.Id ? oldCategory.ImageId2 : categoryEditModel.Image2.Id;

                        if (categoryEditModel.Icon1 != null)
                            entity.IconId1 = oldCategory.IconId1 == categoryEditModel.Icon1.Id ? oldCategory.IconId1 : categoryEditModel.Icon1.Id;

                        if (categoryEditModel.Icon2 != null)
                            entity.IconId2 = oldCategory.IconId2 == categoryEditModel.Icon2.Id ? oldCategory.IconId2 : categoryEditModel.Icon2.Id;

                        var updatedEntity = dbContext.Entry(entity);
                        updatedEntity.State = EntityState.Modified;

                        await dbContext.SaveChangesAsync();

                        if (categoryEditModel.Image1 != null && oldCategory.ImageId1 != categoryEditModel.Image1.Id)
                        {
                            System.IO.File.WriteAllBytes(DirectoryHelper.GetLocalDataPath($"files/{oldCategory.ImageFolderPath}") + $"/{categoryEditModel.Image1.Id}.jpeg", Convert.FromBase64String(categoryEditModel.Image1.File.Substring(categoryEditModel.Image1.File.IndexOf(",") + 1)));
                            System.IO.File.Delete(DirectoryHelper.GetLocalDataPath($"files/{oldCategory.ImageFolderPath}") + $"/{oldCategory.ImageId1}.jpeg");

                        }

                        if (  categoryEditModel.Image2 != null && oldCategory.ImageId2 != categoryEditModel.Image2.Id)
                        {
                            System.IO.File.WriteAllBytes(DirectoryHelper.GetLocalDataPath($"files/{oldCategory.ImageFolderPath}") + $"/{categoryEditModel.Image2.Id}.jpeg", Convert.FromBase64String(categoryEditModel.Image2.File.Substring(categoryEditModel.Image2.File.IndexOf(",") + 1)));
                            System.IO.File.Delete(DirectoryHelper.GetLocalDataPath($"files/{oldCategory.ImageFolderPath}") + $"/{oldCategory.ImageId2}.jpeg");

                        }
                        if (  categoryEditModel.Icon1 != null && oldCategory.IconId1 != categoryEditModel.Icon1.Id)
                        {
                            System.IO.File.WriteAllBytes(DirectoryHelper.GetLocalDataPath($"files/{oldCategory.IconFolderPath}") + $"/{categoryEditModel.Icon1.Id}.jpeg", Convert.FromBase64String(categoryEditModel.Icon1.File.Substring(categoryEditModel.Icon1.File.IndexOf(",") + 1)));
                            System.IO.File.Delete(DirectoryHelper.GetLocalDataPath($"files/{oldCategory.IconFolderPath}") + $"/{oldCategory.IconId1}.jpeg");

                        }

                        if (categoryEditModel.Icon2 != null && oldCategory.IconId2 != categoryEditModel.Icon2.Id )
                        {
                            System.IO.File.WriteAllBytes(DirectoryHelper.GetLocalDataPath($"files/{oldCategory.IconFolderPath}") + $"/{categoryEditModel.Icon2.Id}.jpeg", Convert.FromBase64String(categoryEditModel.Icon2.File.Substring(categoryEditModel.Icon2.File.IndexOf(",") + 1)));
                            System.IO.File.Delete(DirectoryHelper.GetLocalDataPath($"files/{oldCategory.IconFolderPath}") + $"/{oldCategory.IconId2}.jpeg");

                        }

                        return Created("",entity);

                    }
                    else
                    {
                        var entity = new Category
                        {
                            Id = Guid.NewGuid().ToString(),
                            ParentId = categoryEditModel.HasChild ? null : (parentCategory == null ? null : parentCategory.Id),
                            IsActive = categoryEditModel.IsActive,
                            HasChild = categoryEditModel.HasChild,
                            Name = categoryEditModel.Name,
                            Description1 = categoryEditModel.Description1,
                            Description2 = categoryEditModel.Description2,
                            Description3 = categoryEditModel.Description3,
                            ImageFolderPath = imageFolder,
                            IconFolderPath = iconFolder,
                            CreateUser = userId,
                            CreateDate = currDate,
                            UpdateDate = currDate,
                            UpdateUser = userId
                        };



                        if (categoryEditModel.Image1 != null)
                            entity.ImageId1 = categoryEditModel.Image1.Id;

                        if (categoryEditModel.Image2 != null)
                            entity.ImageId2 = categoryEditModel.Image2.Id;

                        if (categoryEditModel.Icon1 != null)
                            entity.IconId1 = categoryEditModel.Icon1.Id;

                        if (categoryEditModel.Icon2 != null)
                            entity.IconId2 = categoryEditModel.Icon2.Id;


                        await dbContext.Categories.AddAsync(entity);

                        await dbContext.SaveChangesAsync();

                        if (categoryEditModel.Image1 != null)
                            System.IO.File.WriteAllBytes(DirectoryHelper.GetLocalDataPath($"files/{imageFolder}") + $"/{categoryEditModel.Image1.Id}.jpeg", Convert.FromBase64String(categoryEditModel.Image1.File.Substring(categoryEditModel.Image1.File.IndexOf(",") + 1)));

                        if(categoryEditModel.Image2!=null)
                            System.IO.File.WriteAllBytes(DirectoryHelper.GetLocalDataPath($"files/{imageFolder}") + $"/{categoryEditModel.Image2.Id}.jpeg", Convert.FromBase64String(categoryEditModel.Image2.File.Substring(categoryEditModel.Image2.File.IndexOf(",") + 1)));
                        
                        if (categoryEditModel.Icon1 != null)
                            System.IO.File.WriteAllBytes(DirectoryHelper.GetLocalDataPath($"files/{imageFolder}") + $"/{categoryEditModel.Icon1.Id}.jpeg", Convert.FromBase64String(categoryEditModel.Icon1.File.Substring(categoryEditModel.Icon1.File.IndexOf(",") + 1)));

                        if (categoryEditModel.Icon2 != null)
                            System.IO.File.WriteAllBytes(DirectoryHelper.GetLocalDataPath($"files/{imageFolder}") + $"/{categoryEditModel.Icon2.Id}.jpeg", Convert.FromBase64String(categoryEditModel.Icon2.File.Substring(categoryEditModel.Icon2.File.IndexOf(",") + 1)));

                        return Created("",entity);
                    }

                    
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost("Update")]
        public async Task<IActionResult> Update()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet("Get")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
               
                using (AppDbContext dbContext = new AppDbContext())
                {
                    var result = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == id);
                    if (result == null) return Ok();

                    var data = new
                    {
                        Id = result.Id,
                        ParentId = result.ParentId,
                        IsActive = result.IsActive,
                        HasChild = result.HasChild,
                        Name = result.Name,
                        Description1 = result.Description1,
                        Description2 = result.Description2,
                        Description3 = result.Description3,
                        Image1 = result.ImageId1 == null ? null : new ImageEditModel
                        {
                            File = $"{ _configuration["Domain:Development"]}category/file/{result.ImageFolderPath}/{result.ImageId1}",
                            Id = result.ImageId1,
                            Order = 1,
                            Selected = false
                        },
                        Image2 = result.ImageId2 == null ? null : new ImageEditModel
                        {
                            File = $"{ _configuration["Domain:Development"]}category/file/{result.ImageFolderPath}/{result.ImageId2}",
                            Id = result.ImageId2,
                            Order = 2,
                            Selected = false
                        },
                        Icon1 = result.IconId1== null ? null : new ImageEditModel
                        {
                            File = $"{ _configuration["Domain:Development"]}category/file/{result.IconFolderPath}/{result.IconId1}",
                            Id = result.IconId1,
                            Order = 1,
                            Selected = false
                        },
                        Icon2 = result.IconId2==null?null: new ImageEditModel
                        {
                            File = $"{ _configuration["Domain:Development"]}category/file/{result.IconFolderPath}/{result.IconId2}",
                            Id = result.IconId2,
                            Order = 2,
                            Selected = false
                        }
                    };

                    
                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("List")]
        //[Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> List()
        {
            try
            {
                
                using (AppDbContext dbContext = new AppDbContext())
                {
                    var data = await dbContext.Categories.ToListAsync();

                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("basic-list")]
        //[Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> BasicList(string id="-1")
        {
            try
            {

                using (AppDbContext dbContext = new AppDbContext())
                {
                    var data = await dbContext.Categories.Where(x=> id != "-1" ? x.ParentId==id: true ).Select(x=>new CategotyBasicDto {
                        Id=x.Id,
                        Name=x.Name,
                        Description=x.Description1,
                        IconUrl = $"{ _configuration["Domain:Development"]}category/file/{x.IconFolderPath}/{x.IconId1}",
                        ImageUrl = $"{ _configuration["Domain:Development"]}category/file/{x.ImageFolderPath}/{x.ImageId1}"
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
        [HttpGet("search-list")]
        //[Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> SearchList(string searchText)
        {
            try
            {

                using (AppDbContext dbContext = new AppDbContext())
                {
                    var data = await dbContext.Categories.Where(x=>x.Name.ToLower().Contains(searchText.ToLower())).Select(x => new CategotyBasicDto
                    {
                        Id = x.Id,
                        Name = x.Name
                       
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
        [HttpGet("search-master-list")]
        //[Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> SearchMasterList(string searchText)
        {
            try
            {

                using (AppDbContext dbContext = new AppDbContext())
                {
                    var data = await dbContext.Categories.Where(x => x.HasChild==true && x.Name.ToLower().Contains(searchText.ToLower())).Select(x => new CategotyBasicDto
                    {
                        Id = x.Id,
                        Name = x.Name

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
    }
}
