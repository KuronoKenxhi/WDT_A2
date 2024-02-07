using System.ComponentModel.DataAnnotations;

namespace s3901335_a2_AdminSite.Models
{
    public class Payee
    {
        public int PayeeID { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        [Required, StringLength(50)]
        public string Address { get; set; }

        [Required, StringLength(40)]
        public string City { get; set; }

        [Required, StringLength(3), RegularExpression("VIC|NSW|TAS|ACT|QLD|WA|SA|NT", ErrorMessage = "Must be a valid Australian State")]
        public string State { get; set; }

        [Required, StringLength(4), RegularExpression("^\\d{4}$", ErrorMessage = "Must be exactly 4 digits")]
        public string PostCode { get; set; }

        [Required, StringLength(14), RegularExpression("^\\(0\\d\\)\\s\\d{4}\\s\\d{4}$", ErrorMessage = "Phone must be of format (0X) XXXX XXXX")]
        public string Phone { get; set; }
    }
}
