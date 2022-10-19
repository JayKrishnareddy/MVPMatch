namespace MVPMatch.ViewModels
{
    public class Login
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
