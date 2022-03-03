using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FOODPEDI.API.REST.DataAccess.Entities
{
    public class Category
    {
        
        [Column(Order = 0),Key]
        public string Id { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;
        public string ParentId { get; set; }
        public bool HasChild { get; set; } = true;

        [Required,StringLength(250)]
        public string Name { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }

        public string ImageFolderPath { get; set; }
        public string IconFolderPath { get; set; }
        public string ImageId1 { get; set; }
        public string ImageId2 { get; set; }
        public string IconId1 { get; set; }
        public string IconId2 { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string CreateUser { get; set; }
        public string UpdateUser { get; set; }

    }
}
