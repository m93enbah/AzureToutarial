using Microsoft.Extensions.Configuration;
using Sqlapp.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

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

        private SqlConnection GetConnection()
        {
            // Here we are creating the SQL connection
            //return new SqlConnection(_configuration.GetConnectionString("SQLConnection"));
            return new SqlConnection(_configuration["App:Setting:Conn"]);

        }

        public IEnumerable<Course> GetCourses()
        {
            List<Course> _lst = new List<Course>();
            string _statement = "SELECT CourseID,CourseName,rating from Course";
            SqlConnection _connection = GetConnection();
            // Let's open the connection
            _connection.Open();
            // We then construct the statement of getting the data from the Course table
            SqlCommand _sqlcommand = new SqlCommand(_statement, _connection);
            // Using the SqlDataReader class , we will read all the data from the Course table
            using (SqlDataReader _reader = _sqlcommand.ExecuteReader())
            {
                while (_reader.Read())
                {
                    Course _course = new Course()
                    {
                        CourseID = _reader.GetInt32(0),
                        CourseName = _reader.GetString(1),
                        Rating = _reader.GetDecimal(2)
                    };

                    _lst.Add(_course);
                }
            }
            _connection.Close();
            return _lst;
        }

    }
    }

    

