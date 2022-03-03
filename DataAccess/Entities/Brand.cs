using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FOODPEDI.API.REST.DataAccess.Entities
{
    public class Brand
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Origin { get; set; }

    }
}
