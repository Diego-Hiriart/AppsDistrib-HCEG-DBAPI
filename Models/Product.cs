namespace AppsDistrib_HCEG_DBAPI.Models
{
    public class Product
    {
        public Product() { }
        
        public Product(int productId, string name, double price)
        {
            this.ProductId = productId;
            this.Name = name;
            this.Price = price;
        }

        public int ProductId { set; get; }
        public string Name { set; get; }
        public double Price { set; get; }
    }
}
