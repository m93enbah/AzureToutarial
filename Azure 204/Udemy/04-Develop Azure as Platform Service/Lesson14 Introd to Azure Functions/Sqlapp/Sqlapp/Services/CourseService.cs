using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Sqlapp.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sqlapp.Services
{
    public class CourseService
    {
        // Ensure to change the below variables to reflect the connection details for your database
        private IConfiguration _configuration;

        public CourseService(IConfiguration _config)
        {
            _configuration = _config;
        }

        public async Task<IEnumerable<Course>> GetCourses()
        {
            var functionUrl = "https://enbehfunc.azurewebsites.net/api/GetCourses?code=iCN5dX5Cl/70HkhhK8/jf30Uob1VaLR665xalriM4sZr89OoAejDsQ==";
            using (HttpClient _client = new HttpClient()) 
            {
                HttpResponseMessage _response = await _client.GetAsync(functionUrl);
                string _content = await _response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<Course>>(_content);
            }
        }
    }
 }

    

