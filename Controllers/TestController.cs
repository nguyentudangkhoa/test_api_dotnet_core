using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using TestDotNet.Models;
using Microsoft.AspNetCore.Hosting;

namespace TestDotNet
{
    [Route("api/[controller]")]
    [ApiController]

    public class TestController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TestController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment){
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public JsonResult Get(){
            string query = @"SELECT * FROM customers";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");

            MySqlDataReader reader;
            using(MySqlConnection mycon = new MySqlConnection(sqlDataSource)){
                mycon.Open();
                using(MySqlCommand mySqlCommand = new MySqlCommand(query, mycon)){
                    reader = mySqlCommand.ExecuteReader();
                    table.Load(reader);

                    reader.Close();
                    mycon.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpGet("{id}")]
        public JsonResult GetSingle(int id){
            string query = @"SELECT name, point FROM customers where id=@id limit 1;";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");

            MySqlDataReader reader;
            using(MySqlConnection mycon = new MySqlConnection(sqlDataSource)){
                mycon.Open();
                using(MySqlCommand mySqlCommand = new MySqlCommand(query, mycon)){
                    mySqlCommand.Parameters.AddWithValue("id", id);
                    reader = mySqlCommand.ExecuteReader();
                    table.Load(reader);

                    reader.Close();
                    mycon.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpPost]
        public JsonResult Post(Customer customer){
            string query = @"INSERT INTO customers(name,point) VALUES(@name, @point);";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");

            MySqlDataReader reader;
            using(MySqlConnection mycon = new MySqlConnection(sqlDataSource))
            {
                mycon.Open();
                using(MySqlCommand mySqlCommand = new MySqlCommand(query, mycon))
                {
                    mySqlCommand.Parameters.AddWithValue("name", customer.Name);
                    mySqlCommand.Parameters.AddWithValue("point", customer.Point);

                    reader = mySqlCommand.ExecuteReader();
                    table.Load(reader);

                    reader.Close();
                    mycon.Close();
                }
            }

            return new JsonResult("Added customer point successful");
        }

        [HttpPut]
        public JsonResult Put(Customer customer){
            string query = @"UPDATE customers
                            SET point=@point, name=@name
                            WHERE id=@id";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");

            MySqlDataReader reader;
            using(MySqlConnection mycon = new MySqlConnection(sqlDataSource))
            {
                mycon.Open();
                using(MySqlCommand mySqlCommand = new MySqlCommand(query, mycon))
                {
                    mySqlCommand.Parameters.AddWithValue("name", customer.Name);
                    mySqlCommand.Parameters.AddWithValue("id", customer.Id);
                    mySqlCommand.Parameters.AddWithValue("point", customer.Point);

                    reader = mySqlCommand.ExecuteReader();
                    table.Load(reader);

                    reader.Close();
                    mycon.Close();
                }
            }

            return new JsonResult("Update customer point successful");
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int id){
            string query = @"DELETE FROM customers
                            WHERE id=@id";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");

            MySqlDataReader reader;
            using(MySqlConnection mycon = new MySqlConnection(sqlDataSource))
            {
                mycon.Open();
                using(MySqlCommand mySqlCommand = new MySqlCommand(query, mycon))
                {
                    mySqlCommand.Parameters.AddWithValue("id", id);

                    reader = mySqlCommand.ExecuteReader();
                    table.Load(reader);

                    reader.Close();
                    mycon.Close();
                }
            }

            return new JsonResult("Delete customer point successful");
        }

        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile(){
            try
            {
                var httpReuest = Request.Form;
                var postedFile = httpReuest.Files[0];
                string filename = postedFile.FileName;
                var physicalPath = _webHostEnvironment.ContentRootPath + "/Photos/" + filename;

                using(var stream=new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return new JsonResult(filename);

            }
            catch(Exception)
            {
                return new JsonResult("anonymous.png");
            }
        }
    }
}