namespace Lab14A.Models.Request
{
    public class REQInsertInvoiceList
    {
        public int IdCustomer { get; set; }
        public List<Invoicev2> Invoices { get; set; }
    }

    public class Invoicev2
    {
        public DateTime Date { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal Total { get; set; }
    }

}
