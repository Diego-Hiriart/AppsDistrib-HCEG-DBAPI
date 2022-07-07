namespace AppsDistrib_HCEG_DBAPI.Models
{
    public class Invoice
    {
        public Invoice() { }

        public Invoice(int invoiceId, int customerId, int orderId, double subtotal, double tax, double total, int paymentMethodId )
        {
            this.InvoiceId = invoiceId;
            this.CustomerId = customerId;
            this.OrderId = orderId;
            this.Subtotal = subtotal;
            this.Tax = tax;
            this.Total = total;
            this.PaymentMethodId = paymentMethodId;
        }

        public int InvoiceId { set; get; }
        public int CustomerId { set; get; }
        public int OrderId { set; get; }
        public double Subtotal { set; get; }
        public double Tax { set; get; }
        public double Total { set; get; }
        public int PaymentMethodId { set; get; }
    }
}
