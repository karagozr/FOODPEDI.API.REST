using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FOODPEDI.API.REST.Models
{
    public class UserResetPassword
    {
        public string Id { get; set; }

        public string Password { get; set; }
    }
}
