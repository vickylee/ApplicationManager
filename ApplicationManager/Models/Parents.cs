using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationManager.Models
{
    public class Parents
    {
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

    }
}
