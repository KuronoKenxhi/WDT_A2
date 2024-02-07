using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace s3901335_a2.Models
{
    public enum LockState
    {
        Unlocked = 0, 
        Locked = 1,
    }
    public class Customer
    {
        [Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CustomerID { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        [StringLength(11), RegularExpression("^\\d{3} \\d{3} \\d{3}$", ErrorMessage = "TFN must be of the format XXX XXX XXX")]
        public string TFN { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        [StringLength(40)]
        public string City { get; set; }

        [RegularExpression("VIC|NSW|QLD|WA|TAS|ACT|NT|SA", ErrorMessage = "State must be a valid Australian State")]
        public string State { get; set; }

        [StringLength(4), RegularExpression("^\\d{4}$", ErrorMessage = "Must be exactly 4 digits")]
        public string PostCode { get; set; }

        [RegularExpression("^04\\d{2} \\d{3} \\d{3}$", ErrorMessage = "Mobile number must be of format 04XX XXX XXX")]
        public string Mobile {  get; set; }

        public LockState LockState { get; set; }

        public virtual List<Account> Accounts { get; set; }

        [NotMapped]
        public Login Login { get; set; }
    }
}
