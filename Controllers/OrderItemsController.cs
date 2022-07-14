using Microsoft.AspNetCore.Mvc;
using System.Data;
using Npgsql;
using System.Diagnostics;
using AppsDistrib_HCEG_DBAPI.Models;
using AppsDistrib_HCEG_DBAPI.Settings;

namespace AppsDistrib_HCEG_DBAPI.Controllers
{
    [ApiController]
    [Route("api/order-items")]
    public class OrderItemsController : ControllerBase
    {
        //A constructor for this class is needed so that when it is called the config and environment info needed are passed
        public OrderItemsController(IConfiguration config, IWebHostEnvironment env)
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
        public async Task<ActionResult<OrderItem>> CreateOrderItem(OrderItem orderItem)
        {
            string createOrderItem = "INSERT INTO \"OrderItems\"(\"OrderId\", \"ProductId\") VALUES(@0, @1)";

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(db))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        using (NpgsqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = createOrderItem;
                            cmd.Parameters.AddWithValue("@0", orderItem.OrderId);//Replace the parameters of the string
                            cmd.Parameters.AddWithValue("@1", orderItem.ProductId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    conn.Close();
                }
                //Create a basic profile when a user is created
                return Ok(orderItem);
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
                return StatusCode(500, new object[] { eSql.Message, orderItem });
            }
        }

        [HttpGet]//Maps this method to the GET request (read)
        public async Task<ActionResult<List<OrderItem>>> ReadOrderItems()
        {
            List<OrderItem> orderItems = new List<OrderItem>();
            string readOrderItems = "SELECT * FROM \"OrderItems\"";
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(db))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        using (NpgsqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = readOrderItems;
                            using (NpgsqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var orderItem = new OrderItem();
                                    orderItem.OrderId = reader.GetInt32(0);
                                    orderItem.ProductId = reader.GetInt32(1);
                                    orderItems.Add(orderItem);//Add order to list
                                }
                            }
                        }
                    }
                    conn.Close();
                }
                return Ok(orderItems);
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
                return StatusCode(500, eSql.Message);
            }
        }

        [HttpGet("search")]//Maps this method to the GET request (read)
        public async Task<ActionResult<List<OrderItem>>> SearchOrderItems(int id)
        {
            List<OrderItem> orderItems = new List<OrderItem>();
            string readOrderItems = "SELECT * FROM \"OrderItems\" WHERE \"OrderId\" = @0";
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(db))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        using (NpgsqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = readOrderItems;
                            cmd.Parameters.AddWithValue("@0", id);
                            using (NpgsqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var orderItem = new OrderItem();
                                    orderItem.OrderId = reader.GetInt32(0);
                                    orderItem.ProductId = reader.GetInt32(1);
                                    orderItems.Add(orderItem);//Add order to list
                                }
                            }
                        }
                    }
                    conn.Close();
                }
                return Ok(orderItems);
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
                return StatusCode(500, eSql.Message);
            }
        }
    }
}
