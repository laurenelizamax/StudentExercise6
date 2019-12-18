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
    public class CohortController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CohortController(IConfiguration config)
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
        /// Gets a list of all Cohorts
        /// </summary>
        /// <returns> A list of Cohorts </returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id AS CohortId, c.CohortName, s.Id AS StudentId, 
                                        s.FirstName AS SFirstName, s.LastName AS SLastName,
                                        s.SlackHandle AS SSlackHandle, s.CohortId, e.Id AS ExerciseId, 
                                        e.ExerciseName, e.ExerciseLanguage,
                                        i.Id AS InstructorId, i.FirstName AS IFirstName, 
                                        i.LastName AS ILastName, i.SlackHandle AS ISlackHandle, 
                                        i.CohortId FROM Cohort c
                                        INNER JOIN Student s ON c.Id = s.CohortId
                                        LEFT JOIN StudentExercise se ON s.Id = se.StudentId
                                        LEFT JOIN Exercise e ON e.Id = se.ExerciseId
                                        LEFT JOIN Instructor i ON c.Id = i.CohortId";
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    List<Cohort> cohorts = new List<Cohort>();

                    while (reader.Read())
                    {
                        var cohortId = reader.GetInt32(reader.GetOrdinal("CohortId"));
                        var alreadyCohort = cohorts.FirstOrDefault(c => c.Id == cohortId);
                        var hasStudents = !reader.IsDBNull(reader.GetOrdinal("StudentId"));
                        var hasInstructors = !reader.IsDBNull(reader.GetOrdinal("InstructorId"));


                        if (alreadyCohort == null)
                        {
                            Cohort cohort = new Cohort
                            {
                                Id = cohortId,
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName")),
                                Students = new List<Student>(),
                                Instructors = new List<Instructor>()
                            };

                            cohorts.Add(cohort);

                            if (hasStudents)
                            {
                                Student student = new Student()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("SFirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("SLastName")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("SSlackHandle")),
                                    Exercise = new List<Exercise>(),
                                    Cohort = new Cohort()
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                        CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                                    }
                                };


                                if (!cohort.Students.Contains(student))
                                {
                                    cohort.Students.Add(student);
                                };


                                if (hasInstructors)
                                {
                                    Instructor instructor = new Instructor()
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("InstructorId")),
                                        FirstName = reader.GetString(reader.GetOrdinal("IFirstName")),
                                        LastName = reader.GetString(reader.GetOrdinal("ILastName")),
                                        SlackHandle = reader.GetString(reader.GetOrdinal("ISlackHandle")),
                                        Cohort = new Cohort()
                                        {
                                            Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                            CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                                        }
                                    };


                                    if (!cohort.Instructors.Contains(instructor))
                                    {
                                        cohort.Instructors.Add(instructor);
                                    }

                                }
                            }


                            else
                            {
                                if (hasStudents)
                                {
                                    Student student = new Student()

                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                        FirstName = reader.GetString(reader.GetOrdinal("SFirstName")),
                                        LastName = reader.GetString(reader.GetOrdinal("SLastName")),
                                        SlackHandle = reader.GetString(reader.GetOrdinal("SSlackHandle")),
                                        CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                        Exercise
                                        = new List<Exercise>(),
                                        Cohort = new Cohort()
                                        {
                                            Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                            CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                                        }
                                    };
                                

                                    if (!alreadyCohort.Students.Exists(s => s.Id == student.Id))
                                    {
                                        alreadyCohort.Students.Add(student);
                                    }
                                }

                                if (hasInstructors)
                                {
                                    Instructor instructor = new Instructor()
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("InstructorId")),
                                        FirstName = reader.GetString(reader.GetOrdinal("IFirstName")),
                                        LastName = reader.GetString(reader.GetOrdinal("ILastName")),
                                        SlackHandle = reader.GetString(reader.GetOrdinal("ISlackHandle")),
                                        CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                        Cohort = new Cohort()
                                        {
                                            Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                            CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                                        }
                                    };

                                    if (!alreadyCohort.Instructors.Exists(i => i.Id == instructor.Id))
                                    {
                                        alreadyCohort.Instructors.Add(instructor);
                                    }
                                }
                            }
                        }
                    }


                        reader.Close();

                        return Ok(cohorts);
                    }
                }
            }
        }
    }

