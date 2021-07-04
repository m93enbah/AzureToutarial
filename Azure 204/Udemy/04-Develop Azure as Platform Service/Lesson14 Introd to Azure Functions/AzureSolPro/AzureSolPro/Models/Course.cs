using Newtonsoft.Json;

namespace AzureSolPro.Models
{
    class Course
    {
        [JsonProperty("courseId")]
        public int CourseID { get; set; }

        [JsonProperty("courseName")]
        public string CourseName { get; set; }

        [JsonProperty("rating")]
        public decimal Rating { get; set; }
    }
}
