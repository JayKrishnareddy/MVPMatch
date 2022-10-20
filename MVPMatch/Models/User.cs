namespace MVPMatch.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public long DespositAmount { get; set; }
        public string? Role { get; set; }
        public bool isActive { get; set; } = true;
    }
}
