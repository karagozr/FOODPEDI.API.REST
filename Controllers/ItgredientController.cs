using FOODPEDI.API.REST.DataAccess;
using FOODPEDI.API.REST.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOODPEDI.API.REST.Controllers
{
    public class IngredientController : BaseController
    {
        [AllowAnonymous]
        [HttpGet("search-list")]
        //[Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> SearchList(string searchText)
        {
            try
            {

                using (AppDbContext dbContext = new AppDbContext())
                {
                    var data = await dbContext.Ingredients.Where(x => x.Name.ToLower().Contains(searchText.ToLower()))
                        .Select(x => new IngredientBasicList
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

    }
}
