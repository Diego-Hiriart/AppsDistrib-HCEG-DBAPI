using Microsoft.AspNetCore.Mvc;
using System.Data;
using Npgsql;
using System.Diagnostics;
using AppsDistrib_HCEG_DBAPI.Models;
using AppsDistrib_HCEG_DBAPI.Settings;

namespace AppsDistrib_HCEG_DBAPI.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        //A constructor for this class is needed so that when it is called the config and evnironment info needed are passed
        public ProductsController(IConfiguration config, IWebHostEnvironment env)
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
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            string createProduct = "INSERT INTO \"Products\"(\"Name\", \"Price\") VALUES(@0, @1)";

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
                            cmd.Parameters.AddWithValue("@0", product.Name);//Replace the parameters of the string
                            cmd.Parameters.AddWithValue("@1", product.Price);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    conn.Close();
                }
                //Create a basic profile when a user is created
                return Ok(product);
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
                return StatusCode(500, new object[] { eSql.Message, product });
            }
        }

        [HttpGet]//Maps this method to the GET request (read)
        public async Task<ActionResult<List<Product>>> ReadProducts()
        {
            List<Product> products = new List<Product>();
            string readProducts = "SELECT * FROM \"Products\"";
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(db))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        using (NpgsqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = readProducts;
                            using (NpgsqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var product = new Product();
                                    //Use castings so that nulls get created if needed
                                    product.ProductId = reader.GetInt32(0);
                                    product.Name = reader[1] as string;
                                    product.Price = reader.GetDouble(2);
                                    products.Add(product);//Add product to list
                                }
                            }
                        }
                    }
                    conn.Close();
                }
                return Ok(products);
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
                return StatusCode(500, eSql.Message);
            }
        }

        [HttpPut]//Maps this method to the Put request (update)
        public async Task<ActionResult<Product>> UpdateProduct(Product product)
        {
            string updateProduct = "UPDATE \"Products\" SET \"Name\"=@0, \"Price\"=@1 WHERE \"ProductId\" = @2";
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
                            cmd.CommandText = updateProduct;
                            cmd.Parameters.AddWithValue("@0", product.Name);//Replace the parameters of the string
                            cmd.Parameters.AddWithValue("@1", product.Price);
                            cmd.Parameters.AddWithValue("@2", product.ProductId);
                            affectedRows = cmd.ExecuteNonQuery();
                        }
                    }
                    conn.Close();
                }
                if (affectedRows > 0)
                {
                    return Ok(product);
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
                return StatusCode(500, new object[] { eSql.Message, product });
            }
            return BadRequest("Product not found");
        }
    }
}
