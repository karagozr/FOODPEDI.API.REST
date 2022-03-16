using FOODPEDI.API.REST.DataAccess;
using FOODPEDI.API.REST.Models;
using FOODPEDI.API.REST.Service;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FOODPEDI.API.REST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IConfiguration _configuration;

        public AdminController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager , RoleManager<AppRole> roleManager, IConfiguration configuration)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
        }



        [HttpGet("user")]
        public async Task<IActionResult> AdminUser(string Id)
        {


            try
            {
                var adminUser = await userManager.Users.Include(x=>x.UserRoles).ThenInclude(ur => ur.Role).Select(x => new {
                    x.UserName,
                    x.FirstName,
                    x.LastName,
                    x.Id,
                }).FirstOrDefaultAsync(x => x.Id == Id);
                

                return Ok(adminUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }





        }

        [HttpGet("users")]
        public async Task<IActionResult> AdminUsers()
        {


            try
            {
                var adminUsers = await userManager.Users.Include(x => x.UserRoles).ThenInclude(ur => ur.Role).Where(x => x.UserRoles.Any(r => r.Role.Name == "Admin")).Select(x => new {
                    x.UserName,
                    x.FirstName,
                    x.LastName,
                    x.Id,
                }).ToListAsync();


                return Ok(adminUsers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }





        }


        [HttpPost("edit-user")]
        public async Task<IActionResult> AdminUsers(UserEditModel userEditModel)
        {

            try
            {
                var userExistId = await userManager.FindByIdAsync(userEditModel.Id);

                var userExistName = await userManager.FindByNameAsync(userEditModel.Username);

                var userExistMail = await userManager.FindByEmailAsync(userEditModel.Email);

                if(userExistId==null && userExistName!=null && userExistMail !=null) return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User already exist" });


                if (userExistId != null)
                {
                    AppUser user = new AppUser
                    {
                        Id = userExistId.Id,
                        Email = userEditModel.Email,
                        SecurityStamp = userExistId.Id,
                        UserName = userEditModel.Username,
                        FirstName = userEditModel.FirstName,
                        LastName = userEditModel.LastName,
                    };


                    var result = await userManager.UpdateAsync(user);

                    if (!result.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User already exist" });
                    }
                    else
                    {
                        return Ok(new { Status = "Success", Message = "Admin User updated" });
                    }

                }
                else
                {
                    AppUser user = new AppUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = userEditModel.Email,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        UserName = userEditModel.Username,
                        FirstName = userEditModel.FirstName,
                        LastName = userEditModel.LastName,
                    };

                    var role = await roleManager.Roles.FirstOrDefaultAsync(x => x.Name == "Admin");
                    if (role == null)
                    {
                        await roleManager.CreateAsync(new AppRole
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Admin",
                            ConcurrencyStamp = "",
                            NormalizedName = "Admin"
                        });
                    }

                    var result = await userManager.CreateAsync(user, userEditModel.Password);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Admin");
                    }



                    if (!result.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User already exist" });
                    }
                    else
                    {
                        return Ok(new { Status = "Success", Message = "Admin User created" });
                    }
                }

                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }





        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(UserResetPassword resetPasswordModel)
        {

            try
            {
                var userExist = await userManager.FindByIdAsync(resetPasswordModel.Id);



                if (userExist != null)
                {


                    var result =  await userManager.RemovePasswordAsync(userExist);
                    if (!result.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User already exist" });
                    }

                    var result1 = await userManager.AddPasswordAsync(userExist, resetPasswordModel.Password);


                    if (!result1.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User already exist" });
                    }


                    return Ok(new { Status = "Success", Message = "Admin User updated" });

                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User not exist" });


                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }





        }


    }
}
