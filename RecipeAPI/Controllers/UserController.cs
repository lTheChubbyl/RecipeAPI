using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RecipeAPI.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RecipeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<UserController> _logger;

        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<UserController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // Register new user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User registered successfully: {UserName}", model.UserName);
                return Ok();
            }

            _logger.LogWarning("User registration failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            return BadRequest(result.Errors);
        }

        // Get user profile
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            _logger.LogInformation("GetProfile() invoked.");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                _logger.LogWarning("User ID not found in token.");
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", userId);
                return NotFound();
            }

            return Ok(new
            {
                userId,
                user.FirstName,
                user.LastName,
                user.UserName,
                user.Email
            });
        }

        // Update user profile
        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                _logger.LogWarning("User ID not found in token.");
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", userId);
                return NotFound();
            }

            // Update profile information
            if (!string.IsNullOrEmpty(model.FirstName))
                user.FirstName = model.FirstName;
            if (!string.IsNullOrEmpty(model.LastName))
                user.LastName = model.LastName;
            if (!string.IsNullOrEmpty(model.UserName))
                user.UserName = model.UserName;
            if (!string.IsNullOrEmpty(model.Email))
                user.Email = model.Email;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                _logger.LogWarning("User update failed: {Errors}", string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                return BadRequest(updateResult.Errors);
            }

            // Handle password change if the new password is provided
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                // Remove the current password (if the user has one)
                var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                if (!removePasswordResult.Succeeded)
                {
                    _logger.LogWarning("Failed to remove current password: {Errors}", string.Join(", ", removePasswordResult.Errors.Select(e => e.Description)));
                    return BadRequest(removePasswordResult.Errors);
                }

                // Add the new password
                var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (!addPasswordResult.Succeeded)
                {
                    _logger.LogWarning("Failed to set new password: {Errors}", string.Join(", ", addPasswordResult.Errors.Select(e => e.Description)));
                    return BadRequest(addPasswordResult.Errors);
                }
            }

            return Ok();
        }

        // Delete user account
        [HttpDelete("delete-profile")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                _logger.LogWarning("User ID not found in token.");
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", userId);
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                _logger.LogInformation("User account deleted successfully: {UserId}", userId);
                return Ok();
            }

            _logger.LogWarning("User deletion failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            return BadRequest(result.Errors);
        }
    }
}
