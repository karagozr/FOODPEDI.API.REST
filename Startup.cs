using FOODPEDI.API.REST.DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOODPEDI.API.REST
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
           
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FOODPEDI.API.REST", Version = "v1" });
            });
            services.AddDbContext<AppDbContext>(option => 
            option.UseNpgsql(Configuration.GetConnectionString("AppConnStr")));

            services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<AppDbContext>();//.AddDefaultTokenProviders();
            //services.ConfigureApplicationCookie(options => { options.LoginPath = "/Auth/Unauthorize";options.AccessDeniedPath = "/Auth/AccessDenied"; });


            using (var context = new AppDbContext())
            {
                context.Database.Migrate();
                var admin = context.Users.FirstOrDefault(x => x.UserName == "Admin");

                var masterRole = context.Roles.FirstOrDefault(x => x.Name == "Master");
                var adminRole = context.Roles.FirstOrDefault(x => x.Name == "Admin");
                var userRole = context.Roles.FirstOrDefault(x => x.Name == "User");

                var masterRoleId = Guid.NewGuid().ToString();
                var adminRoleId = Guid.NewGuid().ToString();
                var userRoleId = Guid.NewGuid().ToString();

                var hasNotRole = (masterRole == null || adminRole == null || userRole == null);

                if (masterRole == null)
                {
                    masterRole = new AppRole
                    {
                        Id = masterRoleId,
                        Name = "Master",
                        NormalizedName = ("Master").ToUpper()

                    };
                    context.Roles.Add(masterRole);
                }

                if (adminRole == null)
                {
                    adminRole = new AppRole
                    {
                        Id = adminRoleId,
                        Name = "Admin",
                        NormalizedName = ("Admin").ToUpper()

                    };
                    context.Roles.Add(adminRole);

                }

                if (userRole == null)
                {
                    userRole = new AppRole
                    {
                        Id = userRoleId,
                        Name = "User",
                        NormalizedName = ("User").ToUpper()

                    };
                    context.Roles.Add(userRole);

                }

                if (hasNotRole)
                    context.SaveChanges();

                if (admin == null)
                {
                    AppUser user = new AppUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = "admin@foodpedi.com",
                        UserName = "Admin",
                        FirstName = "Food",
                        LastName = "Pedi",
                        EmailConfirmed = true
                    };


                    var result = context.Users.Add(user);

                    context.SaveChanges();
                    var _passwordHasher = new PasswordHasher<AppUser>();
                    user.PasswordHash = _passwordHasher.HashPassword(user, "Master1234");
                    user.SecurityStamp = Guid.NewGuid().ToString();

                    context.UserRoles.Add(new AppUserRole
                    {
                        RoleId = masterRole.Id,
                        UserId = user.Id,

                    });

                    context.SaveChanges();

                }
            }

            services.ConfigureApplicationCookie(options =>
            {
                //options.Cookie.Name = "auth_cookie";
                //options.Cookie.SameSite = SameSiteMode.None;
                //options.LoginPath = new PathString("/Auth/Unauthorize");
                //options.AccessDeniedPath = new PathString("/Auth/AccessDenied");

                // Not creating a new object since ASP.NET Identity has created
                // one already and hooked to the OnValidatePrincipal event.
                // See https://github.com/aspnet/AspNetCore/blob/5a64688d8e192cacffda9440e8725c1ed41a30cf/src/Identity/src/Identity/IdentityServiceCollectionExtensions.cs#L56
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };

                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });
            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            services.AddSwaggerGen(config =>
            {
                

                config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                config.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
                });
                //config.DocInclusionPredicate((name, api) => true);
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("MyPolicy");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FOODPEDI.API.REST v1"));
            }

            if (!env.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
