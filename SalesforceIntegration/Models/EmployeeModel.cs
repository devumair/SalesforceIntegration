using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SalesforceIntegration.Models
{
    public class EmployeeModel
    {

        [JsonProperty("Name")]
        public string EmployeeName { get; set; }

        [JsonProperty("Employee_Salary__c")]
        public string EmployeeSalary { get; set; }

        [JsonProperty("Joining_Date__c")]
        public string JoiningDate { get; set; }

        [JsonProperty("Employee_Designation__c")]
        public string EmployeeDesignation { get; set; }

        [JsonProperty("Employee_Image_Details__c")]
        public string EmployeeImageDetails { get; set; }

        [JsonProperty("Employee_Known_Technology__c")]
        public string EmployeeKnownTechnology { get; set; }

        [JsonProperty("Contact_Number__c")]
        public string ContactNumber { get; set; }

        [JsonProperty("CreatedById")]
        public string CreatedBy { get; set; }

        [JsonProperty("LastModifiedById")]
        public string LastModifiedById { get; set; }

        [JsonProperty("OwnerId")]
        public string OwnerId { get; set; }


        public string ToStringQueryCols()
        {
            return string.Join(", ", this.GetType().GetProperties()
                 .Select(p => p.GetCustomAttribute<JsonPropertyAttribute>())
                 .Select(jp => jp.PropertyName));
        }

        public override string ToString()
        {
            return string.Join(", ", this.GetType().GetProperties()
                 .Select(p => $"{p.Name} = {p.GetValue(this, null)}"));
        }

    }
}
