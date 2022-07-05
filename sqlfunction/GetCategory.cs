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

            SqlConnection _connection = GetConnection();

            _connection.Open();

            SqlCommand _sqlcommand = new SqlCommand(_statement, _connection);

            using (SqlDataReader _reader = _sqlcommand.ExecuteReader())
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

            return new OkObjectResult(_catefory_lst);
        }

        private static SqlConnection GetConnection()
        {
            string connectionString = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_SQLConnection");
            return new SqlConnection(connectionString);
        }

        [FunctionName("GetCategory")]
        public static async Task<IActionResult> RunProduct(
            [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req,
            ILogger log)
        {

            
            int CategoryID = int.Parse(req.Query["Id"]);



            string _statement = String.Format("SELECT Id,Name,DisplayOrder,CreatedDateTime from categories WHERE ProductID={0}", CategoryID);

            SqlConnection _connection = GetConnection();

            _connection.Open();

            SqlCommand _sqlcommand = new SqlCommand(_statement, _connection);
            Category _category = new Category();

            try
            {
                using (SqlDataReader _reader = _sqlcommand.ExecuteReader())
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
                return new OkObjectResult(response);
            }
            _connection.Close();

            
        }

    }

}
