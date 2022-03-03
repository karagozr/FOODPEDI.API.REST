using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOODPEDI.API.REST.Models
{
    public class ImageEditModel
    {
        public string Id { get; set; }
        public string File { get; set; }
        public int Order { get; set; }
        public bool Selected { get; set; }
    }
}
