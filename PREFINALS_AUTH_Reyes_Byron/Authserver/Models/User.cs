namespace PREFINALS_AUTH_Reyes_Byron.Authserver.Models
{
    public class User
    {
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
    }
}
