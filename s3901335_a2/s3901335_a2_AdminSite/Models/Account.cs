using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace s3901335_a2_AdminSite.Models
{
    // Create Account type enum
    public enum AccountType
    {
        Checkings = 1,
        Savings = 2
    }
    public class Account
    {
        // not an identity, stop it from auto generating
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Account Number")]
        public int AccountNumber { get; set;}

        [JsonConverter(typeof(AccountTypeStringToAccountTypeEnumConverter))]
        [Required, Display(Name = "Type")]
        public AccountType AccountType { get; set; }

        // Create foreign key reference to customer
        [Required]
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        // Set basic validation for Currency
        [Column(TypeName = "money")]
        [DataType(DataType.Currency)]
        public decimal Balance { get; set; }

        // Ambiguous navigation property
        [InverseProperty("Account")]
        public virtual List<Transaction> Transactions { get; set; }

       
        

    }
}
