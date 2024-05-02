namespace PREFINALS_AUTH_Reyes_Byron.Models
{
    public class UserDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; } // New property
        public string Course { get; set; }
        public string Section { get; set; }
    }
}
