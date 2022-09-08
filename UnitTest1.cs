using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;

namespace RestSharpTest
{
    
    public class Employee
    {
        public int id { get; set; }
        public string name { get; set; }
        public string salary { get; set; }
    }

    [TestClass]
    public class RestSharpCase
    {
        RestClient client;

        [TestInitialize]
        public void Setup()
        {
            client = new RestClient("http://localhost:4000");
        }

        private IRestResponse getEmployeeList()
        {
            //arrange
            RestRequest request = new RestRequest("/employees", Method.GET);

            //act
            IRestResponse response = client.Execute(request);
            return response;
        }

        //UC1:- Ability to Retrieve all Employees in EmployeePayroll JSON Server.
        [TestMethod]
        public void onCallingGETApi_ReturnEmployeeList()
        {
            IRestResponse response = getEmployeeList();
            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.OK);
            List<Employee> dataResponse = JsonConvert.DeserializeObject<List<Employee>>(response.Content);
            Assert.AreEqual(5, dataResponse.Count);

            foreach(Employee e in dataResponse)
            {
                Console.WriteLine("id : " + e.id + " , Name: " + e.name + " Salary: " + e.salary);
            }
        }

        //UC2:Ability to add a new Employee to the EmployeePayroll JSON Server
        [TestMethod]
        public void givenEmployee_OnPost_ShouldReturnAddedEmployee()
        {
            RestRequest request = new RestRequest("/employees", Method.POST);
            JObject jobjectbody = new JObject();
            jobjectbody.Add("name", "Clark");
            jobjectbody.Add("salary", 15000);
            request.AddParameter("application/json", jobjectbody, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.Created);
            Employee dataResponse = JsonConvert.DeserializeObject<Employee>(response.Content);
            Assert.AreEqual("Clark", dataResponse.name);
            Assert.AreEqual("15000", dataResponse.salary);
            Console.WriteLine(response.Content);
        }

        //UC3:- Ability to add multiple Employee to  the EmployeePayroll JSON Server.
        [TestMethod]
        public void GivenMultipleEmployee_OnPost_ThenShouldReturnEmployeeList()
        {
            // Arrange
            List<Employee> employeeList = new List<Employee>();
            employeeList.Add(new Employee { name = "Vinaya", salary = "15000" });
            employeeList.Add(new Employee { name = "Ajaya kumar", salary = "7000" });
            employeeList.Add(new Employee { name = "Powan", salary = "9000" });
            employeeList.Add(new Employee { name = "Swathi", salary = "12000" });
            // Iterate the loop for each employee
            foreach (var emp in employeeList)
            {
                // Initialize the request for POST to add new employee
                RestRequest request = new RestRequest("/employees", Method.POST);
                request.RequestFormat = DataFormat.Json;

                //Added parameters to the request object such as the content-type and attaching the jsonObj with the request
                request.AddBody(emp);

                //Act
                IRestResponse response = client.ExecuteAsync(request).Result;

                //Assert
                Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
                Employee employee = JsonConvert.DeserializeObject<Employee>(response.Content);
                Assert.AreEqual(emp.name, employee.name);
                Assert.AreEqual(emp.salary, employee.salary);
                System.Console.WriteLine(response.Content);
            }
        }

        /*UC4:- Ability to Update Salary in Employee Payroll JSON Server.
               - Firstly Update the Salary in Memory.
               - Post that Use JSON Server and RESTSharp to Update the salary.*/
        [TestMethod]
        public void OnCallingPutAPI_ReturnEmployeeObject()
        {
            // Arrange
            // Initialize the request for PUT to add new employee
            RestRequest request = new RestRequest("/employees/12", Method.PUT);
            request.RequestFormat = DataFormat.Json;

            request.AddBody(new Employee
            {
                name = "Mandara",
                salary = "65000"
            });

            // Act
            IRestResponse response = client.Execute(request);

            // Assert
            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.OK);
            Employee employee = JsonConvert.DeserializeObject<Employee>(response.Content);
            Assert.AreEqual("Mandara", employee.name);
            Assert.AreEqual("65000", employee.salary);
            Console.WriteLine(response.Content);
        }

        //UC5:- Ability to Delete Employee from Employee Payroll JSON Server.
         
        [TestMethod]
        public void OnCallingDeleteAPI_ReturnSuccessStatus()
        {
            // Arrange
            // Initialize the request for PUT to add new employee
            RestRequest request = new RestRequest("/employees/4", Method.DELETE);

            // Act
            RestResponse response = client.ExecuteAsync(request).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Console.WriteLine(response.Content);
        }

    }
}