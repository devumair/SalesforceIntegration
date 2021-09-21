using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesforceIntegration
{
    public class EmployeeEntityRESTApi : RESTApi
    {
        public EmployeeEntityRESTApi() : base("Employee__c")
        {
        }

        public void GetSOjectDetail()
        {
            JObject obj = JObject.Parse(QueryRecord(Client, queryMessage));
        }

        public void GetEmployee()
        {
            //string companyName = "My Company";
            //string queryMessage = $"SELECT Id, Name, Phone, Type FROM Account WHERE Name = '{companyName}'";
            string queryMessage = $"SELECT Name FROM {SObjectName}";

            JObject obj = JObject.Parse(QueryRecord(Client, queryMessage));

            if ((string)obj["totalSize"] == "1")
            {
                // Only one record, use it
                string accountId = (string)obj["records"][0]["Id"];
                string accountPhone = (string)obj["records"][0]["Phone"];
            }
            if ((string)obj["totalSize"] == "0")
            {
                // No record, create an Account
            }
            else
            {
                // Multiple records, either filter further to determine correct Account or choose the first result
            }
        }
    }
}
