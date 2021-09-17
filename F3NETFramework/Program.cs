using F3NETFramework.SForceService;
using Microsoft.Azure.KeyVault.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Protocols;

namespace F3NETFramework
{
    class Program
    {
        protected static string SessionID { get; set; }
        static SforceService _service = null;
        static LoginResult _loginResult = null;

        static void Main(string[] args)
        {

            new QuickstartApiSample().run();

            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            //_service = new SforceService();
            //_loginResult = new LoginResult();
            //Console.Write("Enter username: ");
            //string username = Console.ReadLine();
            //Console.Write("Enter password: ");
            //string password = Console.ReadLine();
            //_loginResult = _service.login(username, password);


            //try
            //{
            //    SessionID = _loginResult.sessionId;
            //    Console.WriteLine("Login Successfully!");
            //    Console.WriteLine("User Info:");
            //    Console.WriteLine("Full Name: {0}", _loginResult.userInfo.userFullName);
            //    Console.WriteLine("Organization: {0}", _loginResult.userInfo.organizationName);
            //    Console.WriteLine("Type: {0}", _loginResult.userInfo.userType);

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("ERROR: ", ex.Message, ex);
            //    throw;
            //}


            Console.ReadKey();
        }
    }
}

class QuickstartApiSample
{
    private SforceService binding;

    public void run()
    {
        // Make a login call 
        if (login())
        {
            // Do a describe global 
            describeGlobalSample();

            // Describe an account object 
            describeSObjectsSample();

            // Retrieve some data using a query 
            querySample();

            // Log out
            logout();
        }
    }

    private bool login()
    {
        Console.Write("Enter username: ");
        string username = Console.ReadLine();
        Console.Write("Enter password: ");
        string password = Console.ReadLine();

        // Create a service object 
        binding = new SforceService();

        // Timeout after a minute 
        binding.Timeout = 60000;

        // Try logging in   
        LoginResult lr;
        try
        {

            Console.WriteLine("\nLogging in...\n");
            lr = binding.login(username, password);
        }

        // ApiFault is a proxy stub generated from the WSDL contract when     
        // the web service was imported 
        catch (SoapException e)
        {
            // Write the fault code to the console 
            Console.WriteLine(e.Code);

            // Write the fault message to the console 
            Console.WriteLine("An unexpected error has occurred: " + e.Message);

            // Write the stack trace to the console 
            Console.WriteLine(e.StackTrace);

            // Return False to indicate that the login was not successful 
            return false;
        }



        // Check if the password has expired 
        if (lr.passwordExpired)
        {
            Console.WriteLine("An error has occurred. Your password has expired.");
            return false;
        }


        /** Once the client application has logged in successfully, it will use
         * the results of the login call to reset the endpoint of the service
         * to the virtual server instance that is servicing your organization
         */
        // Save old authentication end point URL
        String authEndPoint = binding.Url;
        // Set returned service endpoint URL
        binding.Url = lr.serverUrl;

        /** The sample client application now has an instance of the SforceService
         * that is pointing to the correct endpoint. Next, the sample client
         * application sets a persistent SOAP header (to be included on all
         * subsequent calls that are made with SforceService) that contains the
         * valid sessionId for our login credentials. To do this, the sample
         * client application creates a new SessionHeader object and persist it to
         * the SforceService. Add the session ID returned from the login to the
         * session header
         */
        binding.SessionHeaderValue = new SessionHeader();
        binding.SessionHeaderValue.sessionId = lr.sessionId;

        printUserInfo(lr, authEndPoint);

        // Return true to indicate that we are logged in, pointed  
        // at the right URL and have our security token in place.     
        return true;
    }

    private void printUserInfo(LoginResult lr, String authEP)
    {
        try
        {
            GetUserInfoResult userInfo = lr.userInfo;

            Console.WriteLine("\nLogging in ...\n");
            Console.WriteLine("UserID: " + userInfo.userId);
            Console.WriteLine("User Full Name: " +
                userInfo.userFullName);
            Console.WriteLine("User Email: " +
                userInfo.userEmail);
            Console.WriteLine();
            Console.WriteLine("SessionID: " +
                lr.sessionId);
            Console.WriteLine("Auth End Point: " +
                authEP);
            Console.WriteLine("Service End Point: " +
                lr.serverUrl);
            Console.WriteLine();
        }
        catch (SoapException e)
        {
            Console.WriteLine("An unexpected error has occurred: " + e.Message +
                " Stack trace: " + e.StackTrace);
        }
    }

    private void logout()
    {
        try
        {
            binding.logout();
            Console.WriteLine("Logged out.");
        }
        catch (SoapException e)
        {
            // Write the fault code to the console 
            Console.WriteLine(e.Code);

            // Write the fault message to the console 
            Console.WriteLine("An unexpected error has occurred: " + e.Message);

            // Write the stack trace to the console 
            Console.WriteLine(e.StackTrace);
        }
    }

    /**
    * To determine the objects that are available to the logged-in
    * user, the sample client application executes a describeGlobal
    * call, which returns all of the objects that are visible to
    * the logged-in user. This call should not be made more than
    * once per session, as the data returned from the call likely
    * does not change frequently. The DescribeGlobalResult is
    * simply echoed to the console.
    */
    private void describeGlobalSample()
    {
        try
        {
            // describeGlobal() returns an array of object results that  
            // includes the object names that are available to the logged-in user. 
            DescribeGlobalResult dgr = binding.describeGlobal();

            Console.WriteLine("\nDescribe Global Results:\n");
            // Loop through the array echoing the object names to the console             
            for (int i = 0; i < dgr.sobjects.Length; i++)
            {
                Console.WriteLine(dgr.sobjects[i].name);
            }
        }
        catch (SoapException e)
        {
            Console.WriteLine("An exception has occurred: " + e.Message +
                "\nStack trace: " + e.StackTrace);
        }
    }

    /**
    * The following method illustrates the type of metadata
    * information that can be obtained for each object available
    * to the user. The sample client application executes a
    * describeSObject call on a given object and then echoes  
    * the returned metadata information to the console. Object
    * metadata information includes permissions, field types
    * and length and available values for picklist fields
    * and types for referenceTo fields.
    */
    private void describeSObjectsSample()
    {
        Console.Write("\nType the name of the object to " +
            "describe (try Account): ");
        string objectType = Console.ReadLine();
        try
        {

            // Call describeSObjects() passing in an array with one object type name 
            DescribeSObjectResult[] dsrArray =
                  binding.describeSObjects(new string[] { objectType });

            // Since we described only one sObject, we should have only
            // one element in the DescribeSObjectResult array.
            DescribeSObjectResult dsr = dsrArray[0];

            // First, get some object properties                  
            Console.WriteLine("\n\nObject Name: " + dsr.name);

            if (dsr.custom) Console.WriteLine("Custom Object");
            if (dsr.label != null) Console.WriteLine("Label: " + dsr.label);

            // Get the permissions on the object 
            if (dsr.createable) Console.WriteLine("Createable");
            if (dsr.deletable) Console.WriteLine("Deleteable");
            if (dsr.queryable) Console.WriteLine("Queryable");
            if (dsr.replicateable) Console.WriteLine("Replicateable");
            if (dsr.retrieveable) Console.WriteLine("Retrieveable");
            if (dsr.searchable) Console.WriteLine("Searchable");
            if (dsr.undeletable) Console.WriteLine("Undeleteable");
            if (dsr.updateable) Console.WriteLine("Updateable");

            Console.WriteLine("Number of fields: " + dsr.fields.Length);

            // Now, retrieve metadata for each field
            for (int i = 0; i < dsr.fields.Length; i++)
            {
                // Get the field 
                Field field = dsr.fields[i];

                // Write some field properties
                Console.WriteLine("Field name: " + field.name);
                Console.WriteLine("\tField Label: " + field.label);

                // This next property indicates that this  
                // field is searched when using 
                // the name search group in SOSL 
                if (field.nameField)
                    Console.WriteLine("\tThis is a name field.");

                if (field.restrictedPicklist)
                    Console.WriteLine("This is a RESTRICTED picklist field.");

                Console.WriteLine("\tType is: " + field.type.ToString());

                if (field.length > 0)
                    Console.WriteLine("\tLength: " + field.length);

                if (field.scale > 0)
                    Console.WriteLine("\tScale: " + field.scale);

                if (field.precision > 0)
                    Console.WriteLine("\tPrecision: " + field.precision);

                if (field.digits > 0)
                    Console.WriteLine("\tDigits: " + field.digits);

                if (field.custom)
                    Console.WriteLine("\tThis is a custom field.");

                // Write the permissions of this field
                if (field.nillable) Console.WriteLine("\tCan be nulled.");
                if (field.createable) Console.WriteLine("\tCreateable");
                if (field.filterable) Console.WriteLine("\tFilterable");
                if (field.updateable) Console.WriteLine("\tUpdateable");

                // If this is a picklist field, show the picklist values   
                if (field.type.Equals(fieldType.picklist))
                {
                    Console.WriteLine("\tPicklist Values");
                    for (int j = 0; j < field.picklistValues.Length; j++)
                        Console.WriteLine("\t\t" + field.picklistValues[j].value);
                }

                // If this is a foreign key field (reference),     
                // show the values 
                if (field.type.Equals(fieldType.reference))
                {
                    Console.WriteLine("\tCan reference these objects:");
                    for (int j = 0; j < field.referenceTo.Length; j++)
                        Console.WriteLine("\t\t" + field.referenceTo[j]);
                }
                Console.WriteLine("");
            }
        }
        catch (SoapException e)
        {
            Console.WriteLine("An exception has occurred: " + e.Message +
                "\nStack trace: " + e.StackTrace);
        }
        Console.WriteLine("Press ENTER to continue...");
        Console.ReadLine();
    }

    private void querySample()
    {
        String soqlQuery = "SELECT FirstName, LastName FROM Contact";
        try
        {
            QueryResult qr = binding.query(soqlQuery);
            bool done = false;

            if (qr.size > 0)
            {
                Console.WriteLine("Logged-in user can see "
                      + qr.records.Length + " contact records.");

                while (!done)
                {
                    Console.WriteLine("");
                    sObject[] records = qr.records;
                    for (int i = 0; i < records.Length; i++)
                    {
                        //Contact con = (Contact)records[i];
                        //string fName = con.FirstName;
                        //string lName = con.LastName;
                        //if (fullName == null)
                        //    Console.WriteLine("Contact " + (i + 1) + ": " + lName);
                        //else
                        //    Console.WriteLine("Contact " + (i + 1) + ": " + fName
                        //           + " " + lName);
                        //Contact con = (Contact)records[i];
                        //string fullName = con.Name;
                        Console.WriteLine("Contact " + (i + 1) + ": " + records[i]);  
                    }

                    if (qr.done)
                    {
                        done = true;
                    }
                    else
                    {
                        qr = binding.queryMore(qr.queryLocator);
                    }
                }
            }
            else
            {
                Console.WriteLine("No records found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("\nFailed to execute query succesfully," +
                "error message was: \n{0}", ex.Message);
        }
        Console.WriteLine("\nPress ENTER to continue...");
        Console.ReadLine();
    }

    private void queryEmployeeSample()
    {
        String soqlQuery = "SELECT * FROM Employee__c";
        try
        {
            QueryResult qr = binding.query(soqlQuery);
            bool done = false;

            if (qr.size > 0)
            {
                Console.WriteLine("Logged-in user can see "
                      + qr.records.Length + " contact records.");

                while (!done)
                {
                    Console.WriteLine("");
                    sObject[] records = qr.records;
                    for (int i = 0; i < records.Length; i++)
                    {
                        //Contact con = (Contact)records[i];
                        //string fName = con.FirstName;
                        //string lName = con.LastName;
                        //if (fullName == null)
                        //    Console.WriteLine("Contact " + (i + 1) + ": " + lName);
                        //else
                        //    Console.WriteLine("Contact " + (i + 1) + ": " + fName
                        //           + " " + lName);
                        //Contact con = (Contact)records[i];
                        //string fullName = con.Name;
                        Console.WriteLine("Contact " + (i + 1) + ": " + records[i]);
                    }

                    if (qr.done)
                    {
                        done = true;
                    }
                    else
                    {
                        qr = binding.queryMore(qr.queryLocator);
                    }
                }
            }
            else
            {
                Console.WriteLine("No records found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("\nFailed to execute query succesfully," +
                "error message was: \n{0}", ex.Message);
        }
        Console.WriteLine("\nPress ENTER to continue...");
        Console.ReadLine();
    }
}
