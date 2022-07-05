using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;

namespace sqlfunction
{
    public static class AddProduct
    {
        [FunctionName("AddProduct")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Product data = JsonConvert.DeserializeObject<Product>(requestBody);

            SqlConnection connection = GetConnection();

            connection.Open();

            string statement = "INSERT INTO Products(ProductName,Quantity) VALUES(@param2,@param3)";

            using (SqlCommand command = new SqlCommand(statement, connection))
            {
                //command.Parameters.Add("@param1", SqlDbType.Int).Value = data.ProductID;
                command.Parameters.Add("@param2", SqlDbType.VarChar, 1000).Value = data.ProductName;
                command.Parameters.Add("@param3", SqlDbType.Int).Value = data.Quantity;
                command.CommandType = CommandType.Text;

            }

            return new OkObjectResult("Course added");
        }
        private static SqlConnection GetConnection()
        {
            string connectionString = "Server=tcp:func1.database.windows.net,1433;Initial Catalog=func;Persist Security Info=False;User ID=subomi;Password=Theflash456;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            
            return new SqlConnection(connectionString);
        }
    }

    
}
