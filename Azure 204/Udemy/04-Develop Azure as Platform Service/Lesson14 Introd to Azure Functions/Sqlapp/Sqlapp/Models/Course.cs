using Newtonsoft.Json;

namespace Sqlapp.Models
{
    public class Course
    {
        [JsonProperty("courseID")]
        public int CourseID { get; set; }

        [JsonProperty("courseName")]
        public string CourseName { get; set; }

        [JsonProperty("rating")]
        public decimal Rating { get; set; }
    }
}
