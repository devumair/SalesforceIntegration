using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SalesforceIntegration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesforceIntegration
{
    public class EmployeeEntityRESTApi : RESTApi
    {
        const string ENTITY_NAME = "Employee__c";

        public EmployeeEntityRESTApi() : base()
        {
            //GetSOjectDetail();
            //GetEmployee();
            UpdateEmployeeContactNumberSalary();
        }

        public void GetSOjectDetail()
        {
            JObject obj = JObject.Parse(GetSOjectDetail(ENTITY_NAME));

            if ((string)obj["totalSize"] == "1")
            {
                // Only one record, use it
                //string accountId = (string)obj["records"][0]["Id"];
                //string accountPhone = (string)obj["records"][0]["Phone"];
                var employeeData = JsonConvert.DeserializeObject(obj["records"][0].ToString());
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

        public void GetEmployee()
        {
            EmployeeModel employeeData = new EmployeeModel();
            //string companyName = "My Company";
            //string queryMessage = $"SELECT Id, Name, Phone, Type FROM Account WHERE Name = '{companyName}'";
            string queryMessage = $"SELECT {employeeData.ToStringQueryCols()} FROM {ENTITY_NAME}";

            var queryResult = QueryRecord(queryMessage);
            JObject obj = JObject.Parse(queryResult);

            if ((string)obj["totalSize"] == "1")
            {
                // Only one record, use it
                //string accountId = (string)obj["records"][0]["Id"];
                //string accountPhone = (string)obj["records"][0]["Phone"];
                var stringObj = JsonConvert.SerializeObject(obj["records"][0]);
                employeeData = JsonConvert.DeserializeObject<EmployeeModel>(stringObj);

                Console.WriteLine("Employee Info: {0}", employeeData.ToString());
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

        public string UpdateEmployeeContactNumberSalary()
        {
            var employeeData = new EmployeeModel
            {
                EntityID = "a025f000001TwGPAA0",
                ContactNumber = "123-123-1234",
                EmployeeSalary = "51000"
            };

            //string updateMessage = $"<root>" +
            //    $"<Contact_Number__c>{employeeData.ContactNumber}</Contact_Number__c>" +
            //    $"<Employee_Salary__c>{employeeData.EmployeeSalary}</Employee_Salary__c>" +
            //    $"</root>";

            //string updateMessage = JsonConvert.SerializeObject(employeeData, Newtonsoft.Json.Formatting.None,
            //                new JsonSerializerSettings
            //                {
            //                    NullValueHandling = NullValueHandling.Ignore,

            //                });

            string updateMessage = JsonConvert.SerializeObject(employeeData);

            string result = UpdateRecord(updateMessage, ENTITY_NAME, employeeData.EntityID);

            if (result != "")
            {
                // logger.SalesforceError("Update", "Account");
                return null;
            }

            // logger.SalesforceSuccess("Update", "Account", accountId);
            return employeeData.EntityID;
        }

    }
}
