using System.ComponentModel.DataAnnotations.Schema;

namespace MVPMatch.Models
{
    public class Products{
        [Key]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public long Price { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [ForeignKey("User")]
        public int CreatedBy { get; set; }
        public bool isActive { get; set; } = true;
    }
}