using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

using System.Data.SqlClient;

namespace Api1
{
    public static class GetProductByID
    {
        private const string QUERY = "SELECT ProductID, Name, ProductNumber, Color, StandardCost, ListPrice, Size, Weight, SellStartDate, SellEndDate, ModifiedDate " +
                    "FROM [SalesLT].[Product] " +
                    "WHERE ProductID = @id";

        [FunctionName("GetProductByID")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetProductByID/{id:int}")] HttpRequest req,
            int id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            Product product = new Product();

            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand command = new SqlCommand(QUERY, conn))
                {
                    SqlParameter param = command.CreateParameter();
                    param.ParameterName = "@id";
                    param.SqlDbType = System.Data.SqlDbType.Int;
                    param.Direction = System.Data.ParameterDirection.Input;
                    param.Value = id;

                    command.Parameters.Add(param);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            product.ProductID = reader.GetInt32(0);
                            product.Name = reader.GetString(1);
                            product.ProductNumber = reader.GetString(2);
                            product.Color = reader.GetString(3);
                            product.StandardCost = reader.GetDecimal(4);
                            product.ListPrice = reader.GetDecimal(5);

                            if (!reader.IsDBNull(6))
                                product.Size = reader.GetString(6);

                            if (!reader.IsDBNull(7))
                                product.Weight = reader.GetDecimal(7);

                            product.SellStartDate = reader.GetDateTime(8);

                            if (!reader.IsDBNull(9))
                                product.SellEndDate = reader.GetDateTime(9);

                            if (!reader.IsDBNull(10))
                                product.ModifiedDate = reader.GetDateTime(10);
                        }
                    }
                }
            }

            string jsonString = JsonSerializer.Serialize(product);

            return new OkObjectResult(jsonString);
        }
    }
}
