using AutoMapper;
using BookShelve.Api.Domain.Entities;
using BookShelve.Api.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookShelve.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(ILogger<AuthController> logger, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
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
        public async Task<IActionResult> Login(LoginUserDto userDto)
        {
            _logger.LogInformation($"Attempting to log user in with {userDto.UserEmail}");
            try
            {
                var user = await _userManager.FindByEmailAsync(userDto.UserEmail);

                var validPassword = await _userManager.CheckPasswordAsync(user, userDto.Password);

                if (user == null || validPassword == false)
                {
                    return NotFound();
                }

                return Accepted();

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $"Attemp to login user went wrong in the {nameof(Login)}");
                return Problem($"Something went wrong in the {nameof(Login)} method.");
            }
        }
    }
}
