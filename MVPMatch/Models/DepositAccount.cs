using System.ComponentModel.DataAnnotations.Schema;

namespace MVPMatch.Models
{
    public class DepositAccount
    {
        [Key]
        public int DepositId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public bool isActive { get; set; } = true;
        public int Amount { get; set; }
    }
}
