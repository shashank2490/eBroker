namespace Repository.Entities
{
    public class Account: Item
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
    }
}