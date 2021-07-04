using mvcapp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ConsumeAPI
{
    public partial class Consume : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            string path = "api/Course";
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://webapi9000.azurewebsites.net/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                var response = client.GetAsync(path);
                response.Wait();

                var result= response.Result;
                if(result.IsSuccessStatusCode)
                {
                    var CourseGet = result.Content.ReadAsAsync<Course[]>();
                    CourseGet.Wait();

                    var CourseList = CourseGet.Result;
                    
                    foreach(Course obj in CourseList)
                    {
                        Response.Write("Course Name : ");
                        Response.Write(obj.Name);
                        Response.Write("</br>");
                        
                    }
                    
                }
                
            }
        }
    }
}