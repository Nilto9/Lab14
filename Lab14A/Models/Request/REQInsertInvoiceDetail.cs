namespace Lab14A.Models.Request
{
    public class REQInsertInvoiceDetail
    {
        public int IdInvoice { get; set; }
        public List<REQDetail> Details { get; set; }
    }

    public class REQDetail
    {
        public int ProductId { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
    }

}
