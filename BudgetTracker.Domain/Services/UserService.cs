
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Model.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace BudgetTracker.Domain.Services
{
    public interface IUserService
    {
        Task<ApiResponse<UserResponse>> CreateUser(SignUpModel signUp);
        Task<ApiResponse<List<string>>> AssignRoleToUserAsync(List<string> roles, ApplicationUser user);
        Task<ApiResponse<LoginOtpResponse>> OtpLoginAsync(LoginModel loginModel);
        Task<ApiResponse<LoginResponse>> LoginUserWithJWTokenAsync(string otp, string email);
    }

    public class UserService : IUserService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public UserService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ITokenService tokenService, RoleManager<IdentityRole> roleManager, IEmailService emailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
            _roleManager = roleManager;
            _emailService = emailService;
        }

        public async Task<ApiResponse<List<string>>> AssignRoleToUserAsync(List<string> roles, ApplicationUser user)
        {
            var assignRole = new List<string>();
            foreach (var role in roles)
            {
                if (await _roleManager.RoleExistsAsync(role))
                {
                    if (!await _userManager.IsInRoleAsync(user, role))
                    {
                        await _userManager.AddToRoleAsync(user, role);
                        assignRole.Add(role);
                    }
                }
            }
            return new ApiResponse<List<string>> { IsSuccess = true, StatusCode = 200, Message = "Role has been assigned", Response = assignRole };
        }

        public async Task<ApiResponse<UserResponse>> CreateUser(SignUpModel signUp)
        {
            var userExist = await _userManager.FindByEmailAsync(signUp.Email);
            if (userExist != null)
            {
                return new ApiResponse<UserResponse> { IsSuccess = false, StatusCode = 403, Message = "User already exist." };
            }

            ApplicationUser user = new ApplicationUser()
            {
                FullName = signUp.FullName,
                Email = signUp.Email,
                UserName = signUp.Email,
                TwoFactorEnabled = true
            };
            var result = await _userManager.CreateAsync(user, signUp.Password);
            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                return new ApiResponse<UserResponse> { IsSuccess = true, StatusCode = 201, Message = "User created successfully.", Response = new UserResponse() { User = user, Token = token } };

            }
            else
            {
                return new ApiResponse<UserResponse> { IsSuccess = false, StatusCode = 500, Message = "User failed to create." };

            }

        }

        

        public async Task<ApiResponse<LoginResponse>> LoginUserWithJWTokenAsync(string otp, string email)
        {
            var user = await _userManager.FindByNameAsync(email);
            var signIn = await _signInManager.TwoFactorSignInAsync("Email", otp, false, false);
            if (signIn.Succeeded)
            {
                if (user != null)
                {
                    return await _tokenService.GetJwtTokenAsync(user);
                }
            }
            return new ApiResponse<LoginResponse>()
            {

                Response = new LoginResponse()
                {

                },
                IsSuccess = false,
                StatusCode = 400,
                Message = $"Invalid Otp"
            };
        }

        

        public async Task<ApiResponse<LoginOtpResponse>> OtpLoginAsync(LoginModel loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.Email);
            if (user != null)
            {
                await _signInManager.SignOutAsync();
                await _signInManager.PasswordSignInAsync(user, loginModel.Password, false, true);
                if (user.TwoFactorEnabled)
                {
                    var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                    return new ApiResponse<LoginOtpResponse>
                    {
                        Response = new LoginOtpResponse()
                        {
                            User = user,
                            Token = token,
                            IsTwoFactorEnable = user.TwoFactorEnabled
                        },
                        IsSuccess = true,
                        StatusCode = 200,
                        Message = $"OTP sent to user email {user.Email}.",
                    };

                }
                else
                {
                    return new ApiResponse<LoginOtpResponse>
                    {
                        Response = new LoginOtpResponse()
                        {
                            User = user,
                            Token = string.Empty,
                            IsTwoFactorEnable = user.TwoFactorEnabled
                        },
                        IsSuccess = true,
                        StatusCode = 200,
                        Message = $"2FA is not enabled.",
                    };
                }
            }

            else
            {
                return new ApiResponse<LoginOtpResponse>
                {                    
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = $"User does not exist.",
                };
            }

        }
    }
}
