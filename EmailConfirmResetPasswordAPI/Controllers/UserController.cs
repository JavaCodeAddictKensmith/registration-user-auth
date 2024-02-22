using EmailConfirmResetPasswordAPI.DTO;
using EmailConfirmResetPasswordAPI.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmailConfirmResetPasswordAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRegistrationRepository _userRegistration;
    
        public UserController(IUserRegistrationRepository userRegistration)
        {
            _userRegistration = userRegistration;
        }
        /// <summary>
        /// User Registration. Please add all the requirements
        /// </summary>
        /// <param name="userRegister"></param>
        /// <returns>Successful or UnSuccesful</returns>
        [HttpPost("Register")]
        public IActionResult Register([FromQuery]UserRegisterRequest userRegister)
        {
            string message = _userRegistration.RegisterAUser(userRegister);
            return Ok(message);
        }
        [HttpPost("Verify")]
        public IActionResult VerifyToken([FromQuery] string token)
        {
            string message = _userRegistration.Verify(token);
            return Ok(message);
        }
        [HttpPost("Login")]
        public IActionResult Login([FromQuery] UserLoginRequest userLoginRequest)
        {
            string message = _userRegistration.Login(userLoginRequest);
            return Ok(message);
        }
        [HttpPut("Forgot-Password")]
        public IActionResult ForgotPassword([FromQuery]string Email)
        {
            string message = _userRegistration.ForgotPassword(Email);
            return Ok(message);
        }
        [HttpPut("Reset-Password")]
        public IActionResult ResetPassword([FromQuery]UserPasswordResetRequest resetRequest)
        {
            string message = _userRegistration.ResetPassword(resetRequest);
            return Ok(message);
        }
    }
}
