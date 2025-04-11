namespace Domain.Inventory
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int PerItem { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string CurrencyCode { get; set; }

    }
}
