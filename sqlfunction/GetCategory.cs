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
using System.Collections.Generic;

namespace sqlfunction
{
    public static class GetCategory
    {
        [FunctionName("GetCategories")]
        public static async Task<IActionResult> RunProducts(
            [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Get data from the database");
            List<Category> _catefory_lst = new List<Category>();

            string _statement = "SELECT Id,Name,DisplayOrder,CreatedDateTime from categories";

            MySqlConnection _connection = GetConnection();

            _connection.Open();

            MySqlCommand _sqlcommand = new MySqlCommand(_statement, _connection);

            using (MySqlDataReader _reader = _sqlcommand.ExecuteReader())
            {
                while (_reader.Read())
                {
                     Category _category = new Category()
                    {
                        Id = _reader.GetInt32(0),
                        Name = _reader.GetString(1),
                        DisplayOrder = _reader.GetInt32(2),
                        CreatedDateTime = _reader.GetDateTime(3)
                    };

                    _catefory_lst.Add(_category);
                }
            }
            _connection.Close();            

            return new OkObjectResult(JsonConvert.SerializeObject(_catefory_lst));
        }

        private static MySqlConnection GetConnection()
        {
            string connectionString = Environment.GetEnvironmentVariable("MYSQLCONNSTR_SQLConnection");
            return new MySqlConnection(connectionString);
        }

        [FunctionName("GetCategory")]
        public static async Task<IActionResult> RunProduct(
            [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req,
            ILogger log)
        {

            
            int CategoryID = int.Parse(req.Query["Id"]);



            string _statement = String.Format("SELECT Id,Name,DisplayOrder,CreatedDateTime from categories WHERE Id={0}", CategoryID);

            MySqlConnection _connection = GetConnection();

            _connection.Open();

            MySqlCommand _sqlcommand = new SqlCommand(_statement, _connection);
            Category _category = new Category();

            try
            {
                using (MySqlDataReader _reader = _sqlcommand.ExecuteReader())
                {
                    _reader.Read();
                    _category.Id = _reader.GetInt32(0);
                    _category.Name = _reader.GetString(1);
                    _category.DisplayOrder = _reader.GetInt32(2);
                    _category.CreatedDateTime = _reader.GetDateTime(3);

                    var response = _category;
                    return new OkObjectResult(response);
                }
            }
            catch(Exception ex)
            {
                var response = "No Records found";
                return new OkObjectResult(JsonConvert.SerializeObject(response));
            }
            _connection.Close();

            
        }

    }

}
