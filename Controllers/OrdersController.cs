using Microsoft.AspNetCore.Mvc;
using System.Data;
using Npgsql;
using System.Diagnostics;
using AppsDistrib_HCEG_DBAPI.Models;
using AppsDistrib_HCEG_DBAPI.Settings;

namespace AppsDistrib_HCEG_DBAPI.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        //A constructor for this class is needed so that when it is called the config and environment info needed are passed
        public OrdersController(IConfiguration config, IWebHostEnvironment env)
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
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            string createOrder = "INSERT INTO \"Orders\"(\"Date\") VALUES(@0)";

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(db))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        using (NpgsqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = createOrder;
                            cmd.Parameters.AddWithValue("@0", order.Date);//Replace the parameters of the string
                            cmd.ExecuteNonQuery();
                        }
                    }
                    conn.Close();
                }
                //Create a basic profile when a user is created
                return Ok(order);
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
                return StatusCode(500, new object[] { eSql.Message, order });
            }
        }

        [HttpGet]//Maps this method to the GET request (read)
        public async Task<ActionResult<List<Order>>> ReadOrders()
        {
            List<Order> orders = new List<Order>();
            string readOrders = "SELECT * FROM \"Orders\"";
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(db))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        using (NpgsqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = readOrders;
                            using (NpgsqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var order = new Order();
                                    order.OrderId = reader.GetInt32(0);
                                    order.Date = reader.GetDateTime(1);
                                    orders.Add(order);//Add order to list
                                }
                            }
                        }
                    }
                    conn.Close();
                }
                return Ok(orders);
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
                return StatusCode(500, eSql.Message);
            }
        }

        [HttpGet("search")]//Maps this method to the GET request (read)
        public async Task<ActionResult<List<Order>>> SearchOrder(int id)
        {
            Order order = new Order();
            string readOrders = "SELECT * FROM \"Orders\" WHERE \"OrderId\" = @0";
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(db))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        using (NpgsqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = readOrders;
                            cmd.Parameters.AddWithValue("@0", id);
                            using (NpgsqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    order.OrderId = reader.GetInt32(0);
                                    order.Date = reader.GetDateTime(1);
                                }
                            }
                        }
                    }
                    conn.Close();
                }
                return Ok(order);
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
                return StatusCode(500, eSql.Message);
            }
        }
    }
}
