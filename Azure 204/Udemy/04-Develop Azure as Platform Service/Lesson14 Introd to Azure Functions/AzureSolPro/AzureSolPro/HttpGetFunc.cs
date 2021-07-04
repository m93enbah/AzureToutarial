using AzureSolPro.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AzureSolPro
{
    public static class HttpGetFunc
    {
        [FunctionName("HttpGetFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var lst = new List<Course>();
            //we will use the syntax of the as following cutomName_ConnectionString
            var _connection_string = Environment.GetEnvironmentVariable("EnbehDb_ConnectionString");
            var _statement = "select CourseID,CourseName,Rating from Course";
            var _conn = new SqlConnection(_connection_string);
            _conn.Open();
            var _sqlCommand = new SqlCommand(_statement, _conn);

            using (SqlDataReader _reader = _sqlCommand.ExecuteReader()) 
            {
                while (_reader.Read()) 
                {
                    var _course = new Course()
                    {
                        CourseID = _reader.GetInt32(0),
                        CourseName = _reader.GetString(1),
                        Rating = _reader.GetDecimal(2)
                    };
                    lst.Add(_course);
                }
            }
            _conn.Close();
            //var responseMessage = JsonConvert.SerializeObject(lst);
            return new OkObjectResult(lst);
        }
    }
}
