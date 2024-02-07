using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace s3901335_a2_AdminSite.Models
{
    public class Login
    {
        [Column(TypeName = "char")]
        [Required, StringLength(8), RegularExpression("^\\d{8}$", ErrorMessage = "Login ID must be 8 digits")]
        public string LoginID { get; set; }

        // Foreign Key Reference to Customer
        [Required]
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [Required, Column(TypeName = "char")]
        [StringLength(94)]
        public string PasswordHash { get; set; }
    }
}
