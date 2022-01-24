using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Api1
{
    public static class GetProduct
    {
        private const string QUERY = "SELECT ProductID, Name, ProductNumber, Color, StandardCost, ListPrice, Size, Weight, SellStartDate, SellEndDate, ModifiedDate " +
                    "FROM [SalesLT].[Product] " +
                    "WHERE ProductID = @id";

        [FunctionName("GetProduct")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetProduct/{id}")] HttpRequest req,
            [Sql(commandText:QUERY,
                CommandType = System.Data.CommandType.Text,
                Parameters = "@id = {id}",
                ConnectionStringSetting = "SqlConnectionString")] IEnumerable<Product> product,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(product);
        }
    }
}
