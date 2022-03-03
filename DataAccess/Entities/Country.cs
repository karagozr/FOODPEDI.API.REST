using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOODPEDI.API.REST.DataAccess.Entities
{
    public class MadeCountry
    {
        public int Id { get; set; }
        public string Code1 { get; set; }
        public string Code2 { get; set; }
        public string Name { get; set; }
        public string PhoneCode { get; set; }
    }

    public class MadeState
    {
        public int Id { get; set; }
        public int? CountryId { get; set; }
        public string Name { get; set; }
        public string PlateCode { get; set; }
        public string PhoneCode { get; set; }
        public MadeCountry Country { get; set; }
    }
    public class MadeCity
    {
        public int Id { get; set; }
        public int? StateId { get; set; }
        public string Name { get; set; }
        public MadeState State { get; set; }
    }

}
