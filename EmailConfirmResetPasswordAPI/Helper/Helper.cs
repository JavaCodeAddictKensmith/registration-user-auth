using EmailConfirmResetPasswordAPI.Interfaces;
using EmailConfirmResetPasswordAPI.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Security.Cryptography;

namespace EmailConfirmResetPasswordAPI.Helper
{
    public class Helper : IHelper
    {
        private readonly AppsettingEmail _mail;
        public Helper(IServiceProvider provider)
        {

            _mail = provider.GetRequiredService<AppsettingEmail>();
        }
        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        public string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }
        public bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            };
        }
        public void SendVerificationToken(User user)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_mail.UserName));
            email.To.Add(MailboxAddress.Parse(user.Email));
            email.Subject = _mail.EmailSubject;
            email.Body = new TextPart(TextFormat.Plain)
            {
                Text = user.VerificationToken
            };
            var smtp = new SmtpClient();
            smtp.Connect(_mail.EmailHost, _mail.EmailPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mail.UserName, _mail.Password);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
        public void SendPasswordResetToken(User user)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_mail.UserName));
            email.To.Add(MailboxAddress.Parse(user.Email));
            email.Subject = _mail.EmailSubject;
            email.Body = new TextPart(TextFormat.Plain)
            {
                Text = user.PasswordResetToken
            };
            var smtp = new SmtpClient();
            smtp.Connect(_mail.EmailHost, _mail.EmailPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mail.UserName, _mail.Password);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

    }
}
