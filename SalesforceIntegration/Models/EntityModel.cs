using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SalesforceIntegration.Models
{
    public abstract class EntityModel
    {
        [JsonIgnoreAttribute]
        public string EntityID { get; set; }
    }
}
