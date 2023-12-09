namespace Lab14A.Models
{
    public class Detail
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int InvoiceId { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }

        public Product Product { get; set; }
        public Invoice Invoice { get; set; }
    }
}
