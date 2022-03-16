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
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager , RoleManager<AppRole> roleManager, IConfiguration configuration)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
        }


        private dynamic LoginResult(AppUser user)
        {
            var expiresDate = DateTime.UtcNow.AddDays(7);

            return Ok(new
            {
                login = true,
                Token = GenerateToken(user),
                EndTime = expiresDate
            });


            string GenerateToken(AppUser user)
            {

                var roleList = new List<Claim>();
                roleList.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                roleList.Add(new Claim(ClaimTypes.Name, user.UserName.ToString()));
                roleList.Add(new Claim("Fullname", user.FirstName + " " + user.LastName));
                roleList.Add(new Claim(ClaimTypes.Role, user.UserRoles.FirstOrDefault().Role.Name));

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    roleList.ToArray(),
                    expires: DateTime.Now.AddDays(7),
                    signingCredentials: credentials); ;

                return new JwtSecurityTokenHandler().WriteToken(token);

            }
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterModel userRegisterModel)
        {
            

            try
            {
                var userExist = await userManager.FindByNameAsync(userRegisterModel.Username);
                if (userExist != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User already exist" });
                }
                else
                {
                    AppUser user = new AppUser
                    {
                        Id=Guid.NewGuid().ToString(),
                        Email = userRegisterModel.Email,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        UserName = userRegisterModel.Username,
                        FirstName = userRegisterModel.FirstName,
                        LastName = userRegisterModel.LastName,
                    };

                    var role = await roleManager.Roles.FirstOrDefaultAsync(x => x.Name == "User");
                    if (role == null)
                    {
                        await roleManager.CreateAsync(new AppRole
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "User",
                            ConcurrencyStamp = "",
                            NormalizedName = "User"
                        });
                    }

                    var result = await userManager.CreateAsync(user, userRegisterModel.Password);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "User");
                    }
                    
                    

                    if (!result.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User already exist" });
                    }
                    else
                    {
                        return Ok(new { Status = "Success", Message = "User created" });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            

            

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginModel userLoginModel)
        {
            try
            {
                var checkUser = userManager.Users.Include(a=>a.UserRoles).ThenInclude(i=>i.Role).FirstOrDefault(x => x.Email == userLoginModel.Email || x.UserName == userLoginModel.UserName);

                if (checkUser==null)
                {
                    return NotFound();  
                }

                var _loginResult = await signInManager.PasswordSignInAsync(checkUser ,userLoginModel.Password,true,true);

                //var token = Request.Headers["Authorization"];

                ///var token1 = (User as ClaimsPrincipal).FindFirst("access_token").Value

                return LoginResult(checkUser);  

            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
            }
            return BadRequest();

        }


        [HttpPost("google-login")]
        public async Task<IActionResult> Google(UserLoginModel userLoginModel)
        {
            try
            {
                var payload = GoogleJsonWebSignature.ValidateAsync(userLoginModel.AccessToken, new GoogleJsonWebSignature.ValidationSettings()).Result;
                var user = userManager.Users.Where(x => x.Email == payload.Email).FirstOrDefault();
                
                if (user == null)
                {
                    user = new AppUser()
                    {
                        SecurityStamp = Guid.NewGuid().ToString(),
                        UserName = payload.Email,
                        Email = payload.Email,
                        EmailConfirmed = true,
                        FirstName = payload.GivenName,
                        LastName = payload.FamilyName,
                        OAuthSubject = payload.Subject,
                        OAuthIssuer = payload.Issuer
                    };
                    var res = await userManager.CreateAsync(user, "GoogleAuth.123123");
                }

                return LoginResult(user);
                
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
            }
            return BadRequest();

        }

    }
}
