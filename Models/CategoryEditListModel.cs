using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOODPEDI.API.REST.Models
{
    public class CategoryEditListModel:CategoryEditModel
    {
        public bool Insert { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
    }
}
