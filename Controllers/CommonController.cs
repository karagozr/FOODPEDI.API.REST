using FOODPEDI.API.REST.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOODPEDI.API.REST.Controllers
{
    public class CommonController:BaseController
    {
        [AllowAnonymous]
        [HttpGet("GetCountries")]
        public async Task<IActionResult> GetCountries()
        {
            try
            {
               
                using (AppDbContext dbContext = new AppDbContext())
                {
                    var result = await dbContext.MadeCountries.ToListAsync();

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("GetStatesOfCountry")]
        public async Task<IActionResult> GetStatesOfCountry(int CountryId)
        {
            try
            {

                using (AppDbContext dbContext = new AppDbContext())
                {
                    var result = await dbContext.MadeStates.Where(x=>x.CountryId==CountryId).ToListAsync();

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("GetCitiesOfState")]
        public async Task<IActionResult> GetCitiesOfState(int StateId)
        {
            try
            {

                using (AppDbContext dbContext = new AppDbContext())
                {
                    var result = await dbContext.MadeCities.Where(x => x.StateId == StateId).ToListAsync();

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("brand/search-list")]
        public async Task<IActionResult> SearchBrand(string searchText)
        {
            try
            {

                using (AppDbContext dbContext = new AppDbContext())
                {
                    var result = await dbContext.Brands.Where(x => x.Name.ToLower().Contains(searchText.ToLower())).ToListAsync();

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
