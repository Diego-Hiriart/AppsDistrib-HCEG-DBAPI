using Microsoft.AspNetCore.Mvc;
using System.Data;
using Npgsql;
using System.Diagnostics;
using AppsDistrib_HCEG_DBAPI.Models;
using AppsDistrib_HCEG_DBAPI.Settings;

namespace AppsDistrib_HCEG_DBAPI.Controllers
{
    [ApiController]
    [Route("api/payment-methods")]
    public class PaymentMethodsController : ControllerBase
    {
        //A constructor for this class is needed so that when it is called the config and evnironment info needed are passed
        public PaymentMethodsController(IConfiguration config, IWebHostEnvironment env)
        {
            this.config = config;
            this.env = env;
            this.db = new AppSettings(this.config, this.env).DBConn;
        }
        //These configurations and environment info are needed to create a DBConfig instance that has the right connection string depending on whether the app is running on a development or production environment
        private readonly IConfiguration config;
        private readonly IWebHostEnvironment env;
        private string db;//Connection string

        [HttpGet]//Maps this method to the GET request (read)
        public async Task<ActionResult<List<PaymentMethod>>> ReadPaymentMethod()
        {
            List<PaymentMethod> paymentMethods = new List<PaymentMethod>();
            string readPaymentMethods = "SELECT * FROM \"PaymentMethods\"";
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(db))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        using (NpgsqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = readPaymentMethods;
                            using (NpgsqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var paymentMethod = new PaymentMethod();
                                    //Use castings so that nulls get created if needed
                                    paymentMethod.PaymentMethodId = reader.GetInt32(0);
                                    paymentMethod.Description = reader[1] as string;
                                    paymentMethods.Add(paymentMethod);//Add customer to list
                                }
                            }
                        }
                    }
                    conn.Close();
                }
                return Ok(paymentMethods);
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
                return StatusCode(500, eSql.Message);
            }
        }
    }
}
