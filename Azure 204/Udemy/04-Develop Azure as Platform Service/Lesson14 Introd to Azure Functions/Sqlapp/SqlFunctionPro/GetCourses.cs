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
using Sqlapp.Models;
using System.Data.SqlClient;

namespace SqlFunctionPro
{
    public static class GetCourses
    {
        [FunctionName("GetCourses")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,ILogger log)
        {
            var lst = new List<Course>();
            //we will use the syntax of the as following cutomName_ConnectionString
            // var _connection_string = Environment.GetEnvironmentVariable("enbehdb_SQLConnectionString");
            var _connection_strring = "Server=tcp:enbehserver.database.windows.net,1433;Initial Catalog=enbehdb;Persist Security Info=False;User ID=m.enbeh;Password=Mohammed1993$;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            var _statement = "select CourseID,CourseName,Rating from Course";
            var _conn = new SqlConnection(_connection_strring);
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
            return new OkObjectResult(lst);
        }
    }
}
