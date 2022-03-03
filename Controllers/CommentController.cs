using FOODPEDI.API.REST.DataAccess;
using FOODPEDI.API.REST.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FOODPEDI.API.REST.Controllers
{
    public class CommentController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public CommentController(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpGet("list/{itemId}")]
        //[Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> List(string itemId)
        {
            try
            {

                using (AppDbContext dbContext = new AppDbContext())
                {
                    var data = await dbContext.ItemComments.Where(x=>x.ItemId==itemId)
                        .Include(c => c.User)
                        .Select(x => new {
                            Id = x.Id,
                            ParentId = x.ParentId,
                            Comment = x.Comment,
                            Rate = x.Rate,
                            UserId = x.User.Id,
                            CommentDate = x.CommentDate,
                            UserName = x.User.UserName,
                            FullName = x.User.FirstName +" "+x.User.LastName,
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
        [HttpPost("edit")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> EditComment(EditCommentModel editCommentModel)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                using (AppDbContext dbContext = new AppDbContext())
                {
                    var old = await dbContext.ItemComments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == editCommentModel.Id);

                    if (old == null)
                    {
                        var parent = await dbContext.ItemComments.AsNoTracking().FirstOrDefaultAsync(x => x.IsPassive==false && x.Id == editCommentModel.ParentId);
                        dbContext.ItemComments.Add(new DataAccess.Entities.ItemComment
                        {
                            Id = Guid.NewGuid().ToString(),
                            CommentDate = DateTime.Now,
                            IsPassive=false,
                            Comment = editCommentModel.Comment,
                            ItemId = editCommentModel.ItemId,
                            ParentId = editCommentModel.ParentId,
                            Rate = parent == null ? editCommentModel.Rate : 0,
                            UserId = userId
                        });

                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {

                        if (old.UserId != userId) return Forbid();

                        var parent = await dbContext.ItemComments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == editCommentModel.ParentId);

                        old.Rate = editCommentModel.Rate;
                        old.Comment = editCommentModel.Comment;
                        
                        dbContext.ItemComments.Update(old);

                        await dbContext.SaveChangesAsync();
                    }

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("delete")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> DeleteComment(string id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                using (AppDbContext dbContext = new AppDbContext())
                {
                    var comment = await dbContext.ItemComments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    //var childrenComment = await dbContext.ItemCommends.AsNoTracking().Where(x => x.ParentId == id).ToListAsync();

                    comment.IsPassive = true;
                    dbContext.ItemComments.Update(comment);




                    await dbContext.SaveChangesAsync();

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
