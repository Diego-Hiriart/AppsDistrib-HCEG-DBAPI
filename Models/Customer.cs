namespace AppsDistrib_HCEG_DBAPI.Models
{
    public class Customer
    {
        public Customer() { }

        public Customer(int customerId, string idDocument, string fullName, string phone, string email, string address, DateTime birhtdate)
        {
            this.CustomerId = customerId;
            this.IdDocument = idDocument;
            this.FullName = fullName;
            this.Phone = phone;
            this.Email = email;
            this.Address = address;
            this.BirthDate = birhtdate;
        }
        
        public int CustomerId { set; get; }
        public string IdDocument { set; get; }
        public string FullName { set; get; }
        public string Phone { set; get; }
        public string Email { set; get; }
        public string Address { set; get; }
        public DateTime BirthDate { set; get; }
    }
}
