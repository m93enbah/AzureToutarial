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
using Sqlapp.Models;
using System.Data;

namespace SqlFunctionPro
{
    public static class PostCourses
    {
        [FunctionName("PostCourses")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function,  "post", Route = null)] HttpRequest req,ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<Course>(requestBody);

                //we will use the syntax of the as following cutomName_ConnectionString
                //var _connection_string = Environment.GetEnvironmentVariable("enbehdb_SQLConnectionString");
                var _connection_strring = "Server=tcp:enbehserver.database.windows.net,1433;Initial Catalog=enbehdb;Persist Security Info=False;User ID=m.enbeh;Password=Mohammed1993$;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                var _statement = "insert into Course(CourseId,CourseName,Rating) values(@id,@name,@rating)";
                var _conn = new SqlConnection(_connection_strring);

                _conn.Open();

                using (var _command = new SqlCommand(_statement, _conn))
                {
                    _command.Parameters.Add("@id", SqlDbType.Int).Value = data.CourseID;
                    _command.Parameters.Add("@name", SqlDbType.VarChar, 1000).Value = data.CourseName;
                    _command.Parameters.Add("@rating", SqlDbType.Decimal).Value = data.Rating;
                    _command.CommandType = CommandType.Text;
                    _command.ExecuteNonQuery();
                }

                _conn.Close();
            }
            catch { return new BadRequestResult(); }
            return new OkObjectResult("Course Added");
        }
    }
}
