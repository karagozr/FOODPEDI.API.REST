using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOODPEDI.API.REST.Models
{
    public class FileModel
    {
        public string Key { get; set; }
        public IFormFile Image { get; set; }
    }


    public class CategoryEditModel
    {
        public string Id { get; set; }
        public bool IsActive { get; set; }
        public string ParentId { get; set; }
        public bool HasChild { get; set; }
        public string Name { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public ImageEditModel Image1 { get; set; }
        public ImageEditModel Image2 { get; set; }
        public ImageEditModel Icon1 { get; set; }
        public ImageEditModel Icon2 { get; set; }

    }

    
}
