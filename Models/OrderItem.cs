namespace AppsDistrib_HCEG_DBAPI.Models
{
    public class OrderItem
    {
        public OrderItem() { }

        public OrderItem(int orderId, int productId)
        {
            this.OrderId = orderId;
            this.ProductId = productId;
        }

        public int OrderId { set; get; }
        public int ProductId { set; get; }
    }
}
