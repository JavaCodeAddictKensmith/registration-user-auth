using Dapper;
using EmailConfirmResetPasswordAPI.DTO;
using EmailConfirmResetPasswordAPI.Interfaces;
using EmailConfirmResetPasswordAPI.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Data.SqlClient;

namespace EmailConfirmResetPasswordAPI.Services
{
    public class UserRegistrationRepository : IUserRegistrationRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHelper _helper;
        private readonly string conn;
        public UserRegistrationRepository(IConfiguration configuration, IHelper helper)
        {
            _configuration = configuration;
            _helper = helper;
            conn = _configuration.GetSection("ConnectionStrings:VerifyEmail").Value;
        }
        public  string RegisterAUser(UserRegisterRequest userRegisterRequest)
        {
            string message = string.Empty;
            using var connection = new SqlConnection(conn);
            var registered = connection.QueryFirstOrDefault<User>($"Select * From Users Where Email = @Email", new {userRegisterRequest.Email});
            if (registered != null)
            {
                message = "User already Exist";
            }
            else
            {
                _helper.CreatePasswordHash(userRegisterRequest.Password, out byte[] passwordHash, out byte[] passwordSalt);
                var user = new User()
                {
                    FullName = userRegisterRequest.FullName,
                    Email = userRegisterRequest.Email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    VerificationToken = _helper.CreateRandomToken(),
                };
                connection.Open();
                int i = connection.Execute("Insert into Users(FullName,  Email, PasswordHash, PasswordSalt, VerificationToken) " +
                    "Values(@FullName,  @Email, @PasswordHash, @PasswordSalt, @VerificationToken) ", new
                    {
                        user.FullName,
                        user.Email,
                        user.PasswordHash,
                        user.PasswordSalt,
                        user.VerificationToken
                    });
                connection.Close();
                if (i > 0)
                {
                    _helper.SendVerificationToken(user);
                    message = "A user has been added successfully. Please check your mail for verification";
                }
            }
            return message;
        }
        public string Verify(string token)
        {
            string message = string.Empty;
            using var connection = new SqlConnection(conn);
            var user = connection.QueryFirstOrDefault<User>($"Select * From Users Where VerificationToken = @VerificationToken", new { VerificationToken = token } );
            if(user == null)
            {
                message = "Invalid Token";
            }
            else
            {
                user.VerifiedAt = DateTime.Now;
                int i = connection.Execute($"Update Users Set VerifiedAt = @VerifiedAt", new { VerifiedAt  = user.VerifiedAt});
                if(i > 0)
                {
                    message = "User Verified";
                }
            }
            return message;
        }
        public string ForgotPassword(string Email)
        {
            string message = string.Empty ;
            using var connection = new SqlConnection(conn);
            var user = connection.QueryFirstOrDefault<User>("Select * From Users Where Email = @Email", new {Email = Email});
            if(user == null)
            {
                message = "User not found";
            }
            else
            {
                user.PasswordResetToken = _helper.CreateRandomToken();
                user.ResetTokenExpires = DateTime.Now.AddHours(1);
                int i = connection.Execute("Update Users Set PasswordResetToken = @PasswordResetToken, ResetTokenExpires = @ResetTokenExpires",
                   new { PasswordResetToken = user.PasswordResetToken, ResetTokenExpires = user.ResetTokenExpires} );
                _helper.SendPasswordResetToken(user);
                if(i > 0)
                {
                    message = "Check your email to reset your password ";
                }
            }
            return message ;
        }
        public string ResetPassword(UserPasswordResetRequest passwordReset)
        {
            string message = string.Empty;
            using var connection = new SqlConnection(conn);
            var user = connection.QueryFirstOrDefault<User>("Select * From Users Where PasswordResetToken = @PasswordResetToken",
                new { PasswordResetToken = passwordReset.Token});
            if( user == null && DateTime.Now > user.ResetTokenExpires)
            {
                message = "Invalid Token";
            }
            else
            {
                _helper.CreatePasswordHash(passwordReset.Password, out byte[]PasswordHash, out byte[]PasswordSalt);
                user.PasswordHash = PasswordHash;
                user.PasswordSalt = PasswordSalt;
               
                int i = connection.Execute("Update Users Set PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt",
                    new { PasswordHash = user.PasswordHash, PasswordSalt = user.PasswordSalt});
                if(i > 0)
                {
                    message = "Reset password was Successful";
                }
            }
            return message;
        }
        public string Login(UserLoginRequest userLoginRequest)
        {
            string message = string.Empty;
            using var connection = new SqlConnection(conn);
            var user = connection.QueryFirst<User>("Select * From Users Where Email = @Email", userLoginRequest);
            if(user == null)
            {
               message = "User no found";
            }
            else if(user.VerifiedAt > DateTime.Now)
            {
                message = "User not Verified";
            }
            else if (!_helper.VerifyPassword(userLoginRequest.Password, user.PasswordHash, user.PasswordSalt))
            {
                message = "Wrong PassWord";
            }
            else
            {
                message = $"Welcome back {user.FullName}";
            }
            return message;
        }
       
    }
}
