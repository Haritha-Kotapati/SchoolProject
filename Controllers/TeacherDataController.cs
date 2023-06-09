﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SchoolProject.Models;
using MySql.Data.MySqlClient;

namespace SchoolProject.Controllers
{
    public class TeacherDataController : ApiController
    {
        //The database context class which allows us to access our MySQL Database.
        private SchoolDbContext School = new SchoolDbContext();

        ///This Controller will access the teachers table of our School database.
        ///<summary>
        ///Returns a list of Teachers in the system.
        /// </summary>
        /// <param id="teacher">some kind of text to seacrh against the teacher name</param>
        /// <example>GET api/TeacherData/ListTeachers</example>
        /// <returns>
        /// A list of teachers (first names and last names) 
        /// </returns>
        /// 
        [HttpGet]
        public IEnumerable<Teacher> ListTeachers(string SearchKey)
        {
            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            //SQL QUERY
            //cmd.CommandText = "Select * from Teachers";
            cmd.CommandText = "Select * from Teachers where teacherfname like '%" + SearchKey + "%' or teacherlname like '%" + SearchKey + "%' ";
            // Select* from Teachers where lower(teacherfname) like lower(@key) or lower(teacherlname) like lower(@key) or lower(concat(teacherfname, ' ', teacherlname)) like lower(@key)

            //Gather Result Set of Query into a variable
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            //Create an empty list of Teacher Names
            List<Teacher> Teachers = new List<Teacher>{};

            //Loop through each row the Result Set
            while (ResultSet.Read())
            {
                //Access column information by the DB column name as an index
                int TeacherId = (int)ResultSet["teacherid"];
                string TeacherFname = (string)ResultSet["teacherfname"];
                string TeacherLname = (string)ResultSet["teacherlname"];
                string EmployeeNumber  =  (string)ResultSet["employeenumber"];
               // DateTime HireDate = (DateTime)ResultSet["hiredate"];
               // decimal Salary = (decimal)ResultSet["salary"];

                Teacher NewTeacher = new Teacher();
                NewTeacher.TeacherId = TeacherId;
                NewTeacher.TeacherFname = TeacherFname;
                NewTeacher.TeacherLname = TeacherLname;
                NewTeacher.EmployeeNumber = EmployeeNumber;
              //  NewTeacher.HireDate = HireDate;
              //  NewTeacher.Salary = Salary;

                //Add the Teacher Name to the list
                Teachers.Add(NewTeacher);
            }

            //Close the connection between the MYSQL Database and the WebServer
            Conn.Close();

            //Return the final list of teachers names
            return Teachers;

        }

        /// <summary>
        /// Finds a teacher from the database through an id. Non-deterministic.
        /// </summary>
        /// <param name="teacherid">Teacher Id</param>
        /// <returns></returns>
        /// /// <example>GET api/TeacherData/FindTeacher/4 -> {Teacher Object}</example>
        /// 
        [HttpGet]
        [Route("api/TeacherData/FindTeacher/{teacherid}")]
        
        public Teacher FindTeacher(int teacherid)
        {
            Teacher NewTeacher = new Teacher();

            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            //Create SQL QUERY
            string query = "Select * from Teachers where teacherid =@id ";
            cmd.CommandText = query ;
            cmd.Parameters.AddWithValue("@id", teacherid);
            cmd.Prepare();

            //Gather Result Set of Query into a variable
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            while (ResultSet.Read())
            {
                int TeacherId = (int)ResultSet["teacherid"];
                string TeacherFname = (string)ResultSet["teacherfname"];
                string TeacherLname = (string)ResultSet["teacherlname"];
                string EmployeeNumber = (string)ResultSet["employeenumber"];
                DateTime HireDate = (DateTime)ResultSet["hiredate"];
                decimal Salary = (decimal)ResultSet["salary"];

                NewTeacher.TeacherId = TeacherId;
                NewTeacher.TeacherFname = TeacherFname;
                NewTeacher.TeacherLname = TeacherLname;
                NewTeacher.EmployeeNumber = EmployeeNumber;
                NewTeacher.HireDate = HireDate;
                NewTeacher.Salary = Salary;
            }
            Conn.Close();

            return NewTeacher;
        }

        

        /// <summary>
        /// Deletes a profile of teacher into the system
        /// </summary>
        /// <param name="id"></param>
        /// <example>POST : /api/TeacherData/DeleteTeacher/5</example>
        /// <returns>
        /// </returns>
        /// 
        [HttpPost]

        public void DeleteTeacher(int teacherid)

       {
            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            string query = "Delete from Teachers where teacherid=@id";
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@id", teacherid);
            cmd.Prepare();

            cmd.ExecuteNonQuery();

            Conn.Close();
        }

        /// <summary>
        /// Adds a Teaher to the database. Non-deterministic.
        /// </summary>
        /// <param name="NewTeacher">An object with fields that map to the columns of the author's table.</param>
        /// <example>
        /// POST api/Teacher DATA /REQUEST BODY
        /// {
        /// "TeacherFname": "Haritha",
        /// "TeacherLname": "Kotapati",
        /// "EmployeeNumber": "RTU787"
        /// }
        /// </example>

        [HttpPost]
        public void AddTeacher([FromBody]Teacher NewTeacher)
        {
            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            string query = "insert into teachers (teacherfname, teacherlname, employeenumber, hiredate, salary  ) " +
                "values (@TeacherFname, @TeacherLname, @EmployeeNumber, CURRENT_DATE(), @Salary)";
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@TeacherFname", NewTeacher.TeacherFname);
            cmd.Parameters.AddWithValue("@TeacherLname", NewTeacher.TeacherLname);
            cmd.Parameters.AddWithValue("@EmployeeNumber", NewTeacher.EmployeeNumber);
            cmd.Parameters.AddWithValue("@Salary", NewTeacher.Salary);
            cmd.Prepare();

            cmd.ExecuteNonQuery();

            Conn.Close();
        }

        /// <summary>
        /// Update a teacher in the system
        /// </summary>
        /// <param name="teacherid"></param>
        /// <param name="UpdateTeacher"></param>
        /// <example>
        /// POST:api/teacherdata/updateteacher/105/mmmnff
        /// POST DATA / FORM DATA / REQUEST BODY
        /// {
        /// teacherid: 5,
        /// teacherfname: 'Hari',
        /// teacherlname: 'tha'
        /// }
        /// 
        /// curl http://localhost:57743/api/teacherdata/updateteacher/5 -H "Content-Type: application/json" -d @teacher.json
        /// </example>
        [HttpPost]
        [Route("api/teacherdata/updateteacher/{id}")]
        public void UpdateTeacher(int id, [FromBody] Teacher UpdateTeacher)
        {
            //Create an instance of a connection
            MySqlConnection Connection = School.AccessDatabase();

            //Open the connection between the web server and database
            Connection.Open();

            //Establish a new command (query) for our database
            MySqlCommand Command = Connection.CreateCommand();

            //SQL QUERY
            string query = "update teachers set teacherfname=@TeacherFname, " +
                            "teacherlname=@TeacherLname where teacherid=@id";
            Command.CommandText = query;
            Command.Parameters.AddWithValue("@TeacherFname", UpdateTeacher.TeacherFname);
            Command.Parameters.AddWithValue("@TeacherLname", UpdateTeacher.TeacherLname);
            Command.Parameters.AddWithValue("@id", id);
            Command.Prepare();

            Command.ExecuteNonQuery();

            Connection.Close();
        }

        
      
    }

}
