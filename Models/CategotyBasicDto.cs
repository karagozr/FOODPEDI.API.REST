﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOODPEDI.API.REST.Models
{
    public class CategotyBasicDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public string IconUrl { get; set; }
    }
}
