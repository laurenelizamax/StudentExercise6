using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using StudentExAPI6.Models;
using Microsoft.AspNetCore.Http;
using System;


namespace StudentExAPI6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IConfiguration _config;

        public StudentController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        /// <summary>
        /// Gets a list of all Students
        /// </summary>
        /// <returns> A list of Students </returns>
        [HttpGet]
        public async Task<IActionResult> GetAllStudents([FromQuery] string orderBy, string searchTerm)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, FirstName, LastName, 
                                       SlackHandle, CohortId 
                                       FROM Student WHERE 1=1";

                    if(!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        cmd.CommandText += " AND FirstName Like @searchTerm OR LastName LIKE @searchTerm";
                        cmd.Parameters.Add(new SqlParameter(searchTerm, "%" + searchTerm + "%"));
                    }

                    if(orderBy == "asc")
                    {
                        cmd.CommandText += " ORDER BY LastName";
                    }

                    else if(orderBy == "desc")
                    {
                        cmd.CommandText += " ORDER BY LastName DESC";
                    }


                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    List<Student> students = new List<Student>();

                    while (reader.Read())
                    {
                        Student student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))
                        };

                        students.Add(student);
                    }
                    reader.Close();

                    return Ok(students);
                }
            }
        }

//        public async Task<IActionResult> Get([FromQuery] string include)
//        {
//            if (include == "exercises")
//            {
//                var students = await GetStudentsWithExercises();
//                return Ok(students);
//            }
//            else
//            {
//                var students = await GetAllStudents();
//                return Ok(students);
//            }
//        }

//    }
//}


///// <summary>
///// Get Students with Exercises
///// </summary>
///// <returns> A list of Students </returns>
//[HttpGet]
//// Uses include 
//private async Task<List<Student> GetStudentsWithExercises()
//        {
//             using (SqlConnection conn = Connection)
//            {
//                conn.Open();
//                using (SqlCommand cmd = conn.CreateCommand())
//                {
//                    cmd.CommandText = @"SELECT s.Id, s.FirstName, s.LastName, 
//                                       s.SlackHandle, s.CohortId, e.ExerciseName, e.ExerciseLanguage, e.Id as ExerciseId
//                                       FROM Student s
//                                       LEFT JOIN StudentExercise se ON s.Id = se.StudentId
//                                       LEFT JOIN Exercise e ON se.ExerciseId = e.Id";
//                    SqlDataReader reader = await cmd.ExecuteReaderAsync()
//                    List<Student> students = new List<Student>();

//                    while (reader.Read())
//                    {
//                        var id = reader.GetInt32(reader.GetOrdinal("Id"));
//                        var existingStudent = students.FirstOrDefault(s => s.Id == id);

//                        if (existingStudent == null)
//                        {
//                            var newStudent = new Student
//                            {
//                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
//                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
//                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
//                                SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
//                                CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))
//                            };


//students.Add(newStudent);
//                            newStudent.Exercises.Add(new Exercise
//                            {
//                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
//                                ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
//                                ExerciseLanguage = reader.GetString(reader.GetOrdinal("ExerciseLanguage"))

//                            });

//                        }

//                        else
//                        {
//                            existingStudent.Exercises.Add(new Exercise
//                            {
//                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
//                                ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
//                                ExerciseLanguage = reader.GetString(reader.GetOrdinal("ExerciseLanguage"))

//                            });
//                        }
//                        reader.Close();

//                        return Ok(students);
//                    }
//                }
//            }
//        }


    }
}