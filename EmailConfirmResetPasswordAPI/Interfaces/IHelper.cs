using EmailConfirmResetPasswordAPI.Models;

namespace EmailConfirmResetPasswordAPI.Interfaces
{
    public interface IHelper
    {
        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        public string CreateRandomToken();
        public bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt);
        public void SendVerificationToken(User user);
        public void SendPasswordResetToken(User user);
    }
}
