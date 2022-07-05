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
    public static class AddCategory
    {
        [FunctionName("AddCategory")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Category data = JsonConvert.DeserializeObject<Category>(requestBody);

            SqlConnection connection = GetConnection();

            connection.Open();

            string statement = "INSERT INTO categories(Name,DisplayOrder,CreatedDateTime) VALUES(@param2,@param3,@param4)";

            using (SqlCommand command = new SqlCommand(statement, connection))
            {
                //command.Parameters.Add("@param1", SqlDbType.Int).Value = data.ProductID;
                command.Parameters.Add("@param2", SqlDbType.VarChar, 1000).Value = data.Name;
                command.Parameters.Add("@param3", SqlDbType.Int).Value = data.DisplayOrder;
                command.Parameters.Add("@param4", SqlDbType.DateTime2).Value = data.CreatedDateTime;
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();


            }

            return new OkObjectResult("Course added");
        }
        private static SqlConnection GetConnection()
        {
            string connectionString = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_SQLConnection");

            return new SqlConnection(connectionString);
        }
    }

    
}
