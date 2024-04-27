using Azure;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Migrations;
using BudgetTracker.Domain.Model;
using BudgetTracker.Domain.Model.Auth;
using BudgetTracker.Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
namespace BudgetTracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;

        public AuthController(IUserService userService, IEmailService emailService, UserManager<ApplicationUser> userManager, IConfiguration configuration, ITokenService tokenService)
        {
            _userService = userService;
            _emailService = emailService;
            _userManager = userManager;
            _configuration = configuration;
            _tokenService = tokenService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] SignUpModel model)
        {
            var tokenResponse = await _userService.CreateUser(model);

            if (tokenResponse.IsSuccess)
            {
                var confirmation = Url.Action(nameof(ConfirmEmail), "Auth", new { tokenResponse.Response.Token, email = model.Email }, Request.Scheme);
                var message = new Message(new string[] { model.Email }, "Confirmation Email link", confirmation!);
                _emailService.SendEmail(message);

                return StatusCode(StatusCodes.Status200OK,
                            new Domain.Model.Response { Status = "Success", Message = "Email verification sent to your email Successfully" });
            }
            return StatusCode(StatusCodes.Status200OK,
                           new Domain.Model.Response {  Message = tokenResponse.Message, IsSuccess = false });

        }



        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK,
                        new Domain.Model.Response { Status = "Success", Message = "Email verified Successfully" });
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Domain.Model.Response { Status = "Error", Message = "This user does not Exist!" });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var loginOtpResponse = await _userService.OtpLoginAsync(loginModel);
            if (loginOtpResponse.Response != null)
            {
                var user = loginOtpResponse.Response.User;
                if (user.TwoFactorEnabled)
                {
                    var token = loginOtpResponse.Response.Token;
                    var message = new Message(new string[] { user.Email! }, "OTP Confirmation", token);
                    _emailService.SendEmail(message);

                    return StatusCode(StatusCodes.Status200OK,
                     new Domain.Model.Response { IsSuccess = loginOtpResponse.IsSuccess, Status = "Success", Message = $"We have sent an OTP to your Email {user.Email}" });
                }
                if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
                {
                    var serviceResponse = await _tokenService.GetJwtTokenAsync(user);
                    return Ok(serviceResponse);

                }
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("login-2FA")]
        public async Task<IActionResult> LoginWithOTP(string code, string email)
        {
            var jwt = await _userService.LoginUserWithJWTokenAsync(code, email);
            if (jwt.IsSuccess)
            {
                return Ok(jwt);
            }
            return StatusCode(StatusCodes.Status404NotFound,
                new Domain.Model.Response { Status = "Error", Message = $"Invalid Code" });
        }
        
        [HttpPost]
        [Route("Refresh-Token")]
        public async Task<IActionResult> RefreshToken(LoginResponse tokens)
        {
            var jwt = await _tokenService.RenewAccessTokenAsync(tokens);
            if (jwt.IsSuccess)
            {
                return Ok(jwt);
            }
            return StatusCode(StatusCodes.Status404NotFound,
                new Domain.Model.Response { Status = "Success", Message = $"Invalid Code" });
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([Required] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var forgotPassword = Url.Action(nameof(RestPassword), "Auth", new { token, email = user.Email }, Request.Scheme);
                var message = new Message(new string[] { user.Email }, "OTP Confirmation", forgotPassword);
                _emailService.SendEmail(message);
                return StatusCode(StatusCodes.Status404NotFound,
            new Domain.Model.Response { Status = "success", Message = $"Password changed request is sent to your email {user.Email}, Please confirm." });
            }
            return StatusCode(StatusCodes.Status404NotFound,
            new Domain.Model.Response { Status = "Error", Message = $"Cloud not send link to your email." });

        }

        [HttpGet("RestPassword")]
        public IActionResult RestPassword(string email, string token)
        {
            var model = new RestPasswordModel { Email = email, Token = token };
            return Ok(new
            {
                model
            });
        }

        [HttpPost("RestPassword")]
        public async Task<IActionResult> RestPassword([FromBody]RestPasswordModel rest)
        {
            var user = await _userManager.FindByEmailAsync(rest.Email);
            if (user != null)
            {
                var restPassword = await _userManager.ResetPasswordAsync(user, rest.Token, rest.Password);
                if (!restPassword.Succeeded)
                {
                    foreach (var error in restPassword.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return Ok(ModelState);
                }
                return StatusCode(StatusCodes.Status400BadRequest,
                new Domain.Model.Response { Status = "Success", Message = $"Password has been changed." });
            }
            return StatusCode(StatusCodes.Status404NotFound,
                new Domain.Model.Response { Status = "Error", Message = $"Cloud not send link to email, Please try again." });

        }
    }
}
