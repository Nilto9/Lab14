namespace Lab14A.Models.Request
{
    public class REQInsertInvoice
    {
        public int IdCustomer { get; set; }
        public DateTime Date { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal Total { get; set; }
    }

}
