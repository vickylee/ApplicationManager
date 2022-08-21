﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationManager.Models
{
    public class Children
    {
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        public string? gender { get; set; }

        public int? grade { get; set; }

        public List<Pets>? pets { get; set; }
    }
}
