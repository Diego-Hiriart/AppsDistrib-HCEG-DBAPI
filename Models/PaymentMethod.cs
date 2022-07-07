namespace AppsDistrib_HCEG_DBAPI.Models
{
    public class PaymentMethod
    {
        public PaymentMethod() { }

        public PaymentMethod(int paymentMehotdId, string description)
        {
            this.PaymentMethodId = paymentMehotdId;
            this.Description = description;
        }
        
        public int PaymentMethodId { set; get; }
        public string Description { set; get; }
    }
}
