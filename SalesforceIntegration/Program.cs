using System;


namespace SalesforceIntegration
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            new EmployeeEntityRESTApi();

            Console.WriteLine("Press ENTER to exit");
            Console.ReadKey();
        }
    }
}