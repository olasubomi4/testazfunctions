using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using MySql.Data.MySqlClient;



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

            MySqlConnection connection = GetConnection();

            connection.Open();

            string statement = "INSERT INTO categories(Name,DisplayOrder,CreatedDateTime) VALUES(@param2,@param3,@param4)";

            try
            {
                using (MySqlCommand command = new MySqlCommand(statement, connection))
                {
                    //command.Parameters.Add("@param1", SqlDbType.Int).Value = data.ProductID;
                    command.Parameters.AddWithValue("@param2", data.Name);
                    command.Parameters.AddWithValue("@param3", data.DisplayOrder);
                    command.Parameters.AddWithValue("@param4", data.CreatedDateTime);
                   // command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();


                }

                return new OkObjectResult("Course added");
            }
            catch(Exception ex)
            {
                var response = "Error invalid input";
                return new OkObjectResult(response);
            }
        }
        private static MySqlConnection GetConnection()
        {
            string connectionString = Environment.GetEnvironmentVariable("MYSQLCONNSTR_SQLConnection");

            return new MySqlConnection(connectionString);
        }
    }

    
}
