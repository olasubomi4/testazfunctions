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

            string statement = "INSERT INTO categories(Name,DisplayOrder) VALUES(data.Name,data.DisplayOrder)";

            try
            {
                using (MySqlCommand command = new MySqlCommand(statement, connection))
                {
                    //command.Parameters.Add("@param1", SqlDbType.Int).Value = data.ProductID;
                    /*command.Parameters.Add("@param2", SqlDbType.NVarChar, 1000).Value = data.Name;
                    command.Parameters.Add("@param3", SqlDbType.Int,100).Value = data.DisplayOrder;
                    command.Parameters.Add("@param4", SqlDbType.DateTime2,7).Value = data.CreatedDateTime;
                    command.CommandType = CommandType.Text;
                    */
                    command.ExecuteNonQuery();


                }

                return new OkObjectResult("Course adfded");
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
