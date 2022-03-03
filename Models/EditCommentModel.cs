using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOODPEDI.API.REST.Models
{
    public class EditCommentModel
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string ItemId { get; set; }
        public string Comment { get; set; }
        public int Rate { get; set; }
    }
}
