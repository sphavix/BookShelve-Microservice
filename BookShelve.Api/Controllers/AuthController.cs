using AutoMapper;
using BookShelve.Api.Domain.Entities;
using BookShelve.Api.Models.User;
using BookShelve.Api.Static;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookShelve.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(ILogger<AuthController> logger, IMapper mapper, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterUserDto userDto)
        {
            _logger.LogInformation($"Attempting to register user with {userDto.UserEmail}");
            try
            {
                //Map entity to Dto
                var user = _mapper.Map<ApplicationUser>(userDto);
                user.UserName = userDto.UserEmail;
                var result = await _userManager.CreateAsync(user, userDto.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }

                //Add user to a role 
                await _userManager.AddToRoleAsync(user, "User");
                return Accepted();
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex, $"Attemp to register user went wrong in the {nameof(Register)}");
                return Problem($"Something went wrong in the {nameof(Register)} method.");
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginUserDto userDto)
        {
            _logger.LogInformation($"Attempting to log user in with {userDto.UserEmail}");
            try
            {
                var user = await _userManager.FindByEmailAsync(userDto.UserEmail);

                var validPassword = await _userManager.CheckPasswordAsync(user, userDto.Password);

                if (user == null || validPassword == false)
                {
                    return Unauthorized(userDto);
                }

                var token = await GenerateJwtToken(user);

                var response = new AuthResponse
                {
                    Email = userDto.UserEmail,
                    Token = token,
                    UserId = user.Id
                };

                return response;

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $"Attemp to login user went wrong in the {nameof(Login)}");
                return Problem($"Something went wrong in the {nameof(Login)} method.");
            }
        }


        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = roles.Select(x =>
                new Claim(ClaimTypes.Role, x)).ToList();

            //Give claims from Identity
            var userClaims = await _userManager.GetClaimsAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(CustomClaimTypes.Uid, user.Id)
            }.Union(userClaims).Union(roleClaims);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(Convert.ToInt32(_configuration["JwtSettings:Duration"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
