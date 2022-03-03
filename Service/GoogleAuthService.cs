using FOODPEDI.API.REST.DataAccess;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOODPEDI.API.REST.Service
{
    public class GoogleAuthService
    {
        private readonly UserManager<AppUser> userManager;


        public GoogleAuthService(UserManager<AppUser>  userManager)
        {
            this.userManager = userManager;
        }
        
        public async Task<AppUser> Authenticate(Google.Apis.Auth.GoogleJsonWebSignature.Payload payload)
        {
            return await FindUserOrAdd(payload);
        }

        private async Task<AppUser> FindUserOrAdd(Google.Apis.Auth.GoogleJsonWebSignature.Payload payload)
        {
            var user = userManager.Users.Where(x => x.Email == payload.Email).FirstOrDefault();
            if (user == null)
            {
                user = new AppUser()
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = payload.Email,
                    Email = payload.Email,
                    EmailConfirmed = true,
                    FirstName=payload.GivenName,
                    LastName=payload.FamilyName,
                    OAuthSubject = payload.Subject,
                    OAuthIssuer = payload.Issuer
                };
                var res = await userManager.CreateAsync(user, "GoogleAuth.123123");
            }
            
            return user;
        }
    }
}
