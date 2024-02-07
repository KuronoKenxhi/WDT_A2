using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace s3901335_a2.Models
{
    // Define an enum for types of transactions
    public enum TransactionType
    {
        Deposit = 1,
        Withdraw = 2,
        Transfer = 3,
        ServiceFee = 4,
        BillPay = 5
    }
    public class Transaction
    {
        public int TransactionID { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; }

        // Make foreign key reference to account
        [Required, ForeignKey("Account")]
        public int AccountNumber { get; set; }
        public virtual Account Account { get; set; }

        // Make foreign Key reference to account
        [ForeignKey("DestinationAccount")]
        public int? DestinationAccountNumber { get; set; }
        public virtual Account DestinationAccount { get; set; }

        [Required, Column(TypeName = "money"), Range(0, double.MaxValue, ErrorMessage = "Must be a positive number")]
        public decimal Amount { get; set; }

        [StringLength(30)]
        public string Comment { get; set; }

        [Required]
        public DateTime TransactionTimeUtc { get; set; }
    }
}
