using Microsoft.AspNetCore.Mvc;
using System.Data;
using Npgsql;
using System.Diagnostics;
using AppsDistrib_HCEG_DBAPI.Models;
using AppsDistrib_HCEG_DBAPI.Settings;

namespace AppsDistrib_HCEG_DBAPI.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        //A constructor for this class is needed so that when it is called the config and evnironment info needed are passed
        public CustomersController(IConfiguration config, IWebHostEnvironment env)
        {
            this.config = config;
            this.env = env;
            this.db = new AppSettings(this.config, this.env).DBConn;
        }
        //These configurations and environment info are needed to create a DBConfig instance that has the right connection string depending on whether the app is running on a development or production environment
        private readonly IConfiguration config;
        private readonly IWebHostEnvironment env;
        private string db;//Connection string

        [HttpPost]//Maps method to Post request
        public async Task<ActionResult<Customer>> CreateCustomer(Customer customer)
        {
            string createCustomer = "INSERT INTO \"Customers\"(\"IdDocument\", \"FullName\", \"Phone\", \"Email\", \"Address\", \"BirthDate\") VALUES(@0, @1, @2, @3, @4, @5)";

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(db))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        using (NpgsqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = createCustomer;
                            cmd.Parameters.AddWithValue("@0", customer.IdDocument);//Replace the parameters of the string
                            cmd.Parameters.AddWithValue("@1", customer.FullName);
                            cmd.Parameters.AddWithValue("@2", customer.Phone);
                            cmd.Parameters.AddWithValue("@3", customer.Email);
                            cmd.Parameters.AddWithValue("@4", customer.Address);
                            cmd.Parameters.AddWithValue("@5", customer.BirthDate);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    conn.Close();
                }
                //Create a basic profile when a user is created
                return Ok(customer);
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
                return StatusCode(500, new object[] { eSql.Message, customer });
            }
        }

        [HttpGet]//Maps this method to the GET request (read)
        public async Task<ActionResult<List<Customer>>> ReadCustomers()
        {
            List<Customer> customers = new List<Customer>();
            string readCustomers = "SELECT * FROM \"Customers\"";
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(db))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        using (NpgsqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = readCustomers;
                            using (NpgsqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var customer = new Customer();
                                    //Use castings so that nulls get created if needed
                                    customer.FullName = reader[0] as string;
                                    customer.Phone = reader[1] as string;
                                    customer.Email = reader[2] as string;
                                    customer.Address = reader[3] as string;
                                    customer.BirthDate = reader.GetDateTime(4);
                                    customer.CustomerId = reader.GetInt32(5);//Get an int from the first column
                                    customer.IdDocument = reader[6] as string;
                                    customers.Add(customer);//Add customer to list
                                }
                            }
                        }
                    }
                    conn.Close();
                }
                return Ok(customers);
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
                return StatusCode(500, eSql.Message);
            }
        }

        [HttpGet("search")]//Maps this method to the GET request (read)
        public async Task<ActionResult<List<Customer>>> SearchCustomer(int id)//Uses the value from the search params (?=id)
        {
            Customer customer = new Customer();
            string readCustomers = "SELECT * FROM \"Customers\" WHERE \"CustomerId\" = @0";
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(db))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        using (NpgsqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = readCustomers;
                            cmd.Parameters.AddWithValue("@0", id);
                            using (NpgsqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    //Use castings so that nulls get created if needed
                                    customer.FullName = reader[0] as string;
                                    customer.Phone = reader[1] as string;
                                    customer.Email = reader[2] as string;
                                    customer.Address = reader[3] as string;
                                    customer.BirthDate = reader.GetDateTime(4);
                                    customer.CustomerId = reader.GetInt32(5);//Get an int from the first column
                                    customer.IdDocument = reader[6] as string;
                                }
                            }
                        }
                    }
                    conn.Close();
                }
                return Ok(customer);
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
                return StatusCode(500, eSql.Message);
            }
        }

        [HttpPut]//Maps this method to the Put request (update)
        public async Task<ActionResult<Customer>> UpdateCustomer(Customer customer)
        {
            string updateCustomer = "UPDATE \"Customers\" SET \"IdDocument\"=@0, \"FullName\"=@1, \"Phone\"=@2, \"Email\"=@3, \"Address\"=@4, \"BirthDate\"=@5 WHERE \"CustomerId\" = @6";
            try
            {
                int affectedRows = 0;
                using (NpgsqlConnection conn = new NpgsqlConnection(db))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        using (NpgsqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = updateCustomer;
                            cmd.Parameters.AddWithValue("@0", customer.IdDocument);//Replace the parameters of the string
                            cmd.Parameters.AddWithValue("@1", customer.FullName);
                            cmd.Parameters.AddWithValue("@2", customer.Phone);
                            cmd.Parameters.AddWithValue("@3", customer.Email);
                            cmd.Parameters.AddWithValue("@4", customer.Address);
                            cmd.Parameters.AddWithValue("@5", customer.BirthDate);
                            cmd.Parameters.AddWithValue("@6", customer.CustomerId);
                            affectedRows = cmd.ExecuteNonQuery();
                        }
                    }
                    conn.Close();
                }
                if (affectedRows > 0)
                {
                    return Ok(customer);
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
                return StatusCode(500, new object[] { eSql.Message, customer });
            }
            return BadRequest("Customer not found");
        }
    }
}
