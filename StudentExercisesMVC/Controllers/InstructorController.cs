using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudentExercisesMVC.Models;

namespace StudentExercisesMVC.Controllers
{
    public class InstructorController : Controller
    {
        private readonly IConfiguration _config;

        public InstructorController(IConfiguration config)
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

        // GET: Instructor
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, FirstName, LastName, CohortId, SlackHandle, Specialty FROM Instructor";

                    var reader = cmd.ExecuteReader();
                    var instructors = new List<Instructor>();

                    while (reader.Read())
                    {
                        var instructor = new Instructor()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Specialty = reader.GetString(reader.GetOrdinal("Specialty"))
                        };
                        instructors.Add(instructor);
                    }
                    reader.Close();
                    return View(instructors);
                }
            }

        }

        // GET: Instructor/Details/5
        public ActionResult Details(int id)
        {
            var instructor = GetInstructorById(id);
            return View(instructor);
        

    }

        // GET: Instructor/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Instructor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Instructor instructor)
        {
            try
            {
                // TODO: Add insert logic here
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Instructor (FirstName, LastName, SlackHandle, CohortId, Specialty)
                                            OUTPUT INSERTED.Id
                                            VALUES (@firstName, @lastName, @slackHandle, @cohortId, @specialty)";

                        cmd.Parameters.Add(new SqlParameter("@firstName", instructor.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", instructor.LastName));
                        cmd.Parameters.Add(new SqlParameter("@slackHandle", instructor.SlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@cohortId", instructor.CohortId));
                        cmd.Parameters.Add(new SqlParameter("@specialty", instructor.Specialty));

                        var id = (int)cmd.ExecuteScalar();
                        instructor.Id = id;

                        return RedirectToAction(nameof(Index));
                    }
                }


            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: Instructor/Edit/5
        public ActionResult Edit(int id)
        {
            var instructor = GetInstructorById(id);
            return View(instructor);
        }

        // POST: Instructor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Instructor instructor)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Instructor 
                                            SET FirstName = @firstName, 
                                                LastName = @lastName, 
                                                SlackHandle = @slackHandle, 
                                                CohortId = @cohortId,
                                                Specialty = @Specialty
                                            WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@firstName", instructor.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", instructor.LastName));
                        cmd.Parameters.Add(new SqlParameter("@slackHandle", instructor.SlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@cohortId", instructor.CohortId));
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@specialty", instructor.Specialty));

                        var rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected < 1)
                        {
                            return NotFound();
                        }
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Instructor/Delete/5
        public ActionResult Delete(int id)
        {
            var instructor = GetInstructorById(id);
            return View(instructor);
        }

        // POST: Instructor/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Instructor instructor)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM Instructor WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        private Instructor GetInstructorById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, FirstName, LastName, CohortId, SlackHandle, Specialty FROM Instructor WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();
                    Instructor instructor = null;

                    if (reader.Read())
                    {
                        instructor = new Instructor()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Specialty = reader.GetString(reader.GetOrdinal("Specialty"))
                        };

                    }
                    reader.Close();
                    return instructor;
                }
            }
        }
    

    }
}