using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace s3901335_a2_AdminSite.Models
{
    public enum Period
    {
        OneOff = 1,
        Monthly = 2
    }
    public class BillPay
    {
        [Required]
        public int BillPayID { get; set; }

        [Required, ForeignKey(nameof(Account))]
        public int AccountNumber { get; set; }
        public virtual Account Account { get; set; }

        [Required]
        public int PayeeID { get; set; }
        public virtual Payee Payee { get; set; }

        [Required, Column(TypeName = "money"), Range(0, double.MaxValue, ErrorMessage = "{0} must be a positive number")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime ScheduleTimeUtc { get; set; }

        [Required]
        public Period Period { get; set; }

    }
}
