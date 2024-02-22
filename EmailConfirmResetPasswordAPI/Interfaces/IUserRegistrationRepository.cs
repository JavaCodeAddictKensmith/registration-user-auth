using EmailConfirmResetPasswordAPI.DTO;
using EmailConfirmResetPasswordAPI.Models;

namespace EmailConfirmResetPasswordAPI.Interfaces
{
    public interface IUserRegistrationRepository
    {
        public string RegisterAUser(UserRegisterRequest userRegisterRequest);
        public string Login(UserLoginRequest userLoginRequest);
        public string Verify(string token);
        public string ForgotPassword(string Email);
        public string ResetPassword(UserPasswordResetRequest passwordReset);
    }
}
