namespace AppsDistrib_HCEG_DBAPI.Models
{
    public class Order
    {
        public Order() { }

        public Order(int orderId, DateTime date)
        {
            this.OrderId = orderId;
            this.Date = date;
        }

        public int OrderId { set; get; }
        public DateTime Date { set; get; }
    }
}
