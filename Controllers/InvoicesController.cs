using Microsoft.AspNetCore.Mvc;
using System.Data;
using Npgsql;
using System.Diagnostics;
using AppsDistrib_HCEG_DBAPI.Models;
using AppsDistrib_HCEG_DBAPI.Settings;

namespace AppsDistrib_HCEG_DBAPI.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoicesController : ControllerBase
    {
        //A constructor for this class is needed so that when it is called the config and environment info needed are passed
        public InvoicesController(IConfiguration config, IWebHostEnvironment env)
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
        public async Task<ActionResult<Product>> CreateInvoice(Invoice invoice)
        {
            string createProduct = "INSERT INTO \"Invoices\"(\"CustomerId\", \"OrderId\", \"Subtotal\", \"Tax\", \"Total\", \"PaymentMethodId\") " +
                "VALUES(@0, @1, @2, @3, @4, @5)";
            
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(db))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        using (NpgsqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = createProduct;
                            cmd.Parameters.AddWithValue("@0", invoice.CustomerId);
                            cmd.Parameters.AddWithValue("@1", invoice.OrderId);
                            cmd.Parameters.AddWithValue("@2", invoice.Subtotal);
                            cmd.Parameters.AddWithValue("@3", invoice.Tax);
                            cmd.Parameters.AddWithValue("@4", invoice.Total);
                            cmd.Parameters.AddWithValue("@5", invoice.PaymentMethodId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    conn.Close();
                }
                //Create a basic profile when a user is created
                return Ok(invoice);
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
                return StatusCode(500, new object[] { eSql.Message, invoice });
            }
        }

        [HttpGet]//Maps this method to the GET request (read)
        public async Task<ActionResult<List<Product>>> ReadInvoices()
        {
            List<Invoice> invoices = new List<Invoice>();
            string readInvoices = "SELECT * FROM \"Invoices\"";
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(db))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        using (NpgsqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = readInvoices;
                            using (NpgsqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var invoice = new Invoice();
                                    //Use castings so that nulls get created if needed
                                    invoice.InvoiceId = reader.GetInt32(0);
                                    invoice.CustomerId = reader.GetInt32(1);
                                    invoice.OrderId = reader.GetInt32(2);
                                    invoice.Subtotal = reader.GetDouble(3);
                                    invoice.Tax = reader.GetDouble(4);
                                    invoice.Total = reader.GetDouble(5);
                                    invoice.PaymentMethodId = reader.GetInt32(6);
                                    invoices.Add(invoice);//Add product to list
                                }
                            }
                        }
                    }
                    conn.Close();
                }
                return Ok(invoices);
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
                return StatusCode(500, eSql.Message);
            }
        }
    }
}
