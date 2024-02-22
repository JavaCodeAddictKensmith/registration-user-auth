namespace EmailConfirmResetPasswordAPI.Models
{
    public class AppsettingEmail
    {
        public string EmailHost { get; set; } = String.Empty;
        public int EmailPort { get; set; }
        public string EmailSubject { get; set; } = String.Empty ;
        public string UserName { get; set; } = String.Empty;  
        public string Password { get; set; } = String .Empty ;
    }
}
