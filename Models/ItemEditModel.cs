using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FOODPEDI.API.REST.Models
{
    public class ItemEditModel
    {
       
      



        public string Id { get; set; }
        
        [Required]
        public string[] Categories { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public BrandModel Brand { get; set; }
        public string Keywords { get; set; }

        [Required]
        public string ShortDescription1 { get; set; }

        [Required]
        public string Description1 { get; set; }
        public string WebPageUrl1 { get; set; }
        public string Barcode1 { get; set; }
        public string Barcode2 { get; set; }
        public double ItemWeight { get; set; }
        public int? MadeCountryId { get; set; }
        public int? MadeStateId { get; set; }
        public int? MadeCityId { get; set; }
        public IEnumerable<IngredientModel> Ingredients { get; set; }
        
        [Required]
        public IEnumerable<ImageEditModel> Images { get; set; }

    }



    public class IngredientModel
    {
        public string IngredientId { get; set; }
        public string IngredientName { get; set; }
        public double Value { get; set; }
        public string UnitCode { get; set; }
    }

    public class BrandModel
    {
        public string Id { get; set; }
        public string Name { get; set; }

    }
}
