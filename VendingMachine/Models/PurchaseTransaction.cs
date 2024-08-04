using System.ComponentModel.DataAnnotations.Schema;

namespace VendingMachine.Models
{
    public class PurchaseTransaction
    {
        public int TransactionID { get; set; }  
        public int CustomerID { get; set; }
        [ForeignKey("CustomerID")]
        public Account Customer { get; set; }

        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public Product Product { get; set; }

        public int Quantity { get; set; }
        public double TotalPrice { get; set; }  
        public DateTime TransactionDate { get; set; }
    }
}
