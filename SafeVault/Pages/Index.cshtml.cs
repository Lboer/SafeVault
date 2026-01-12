using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SafeVault.Pages;

public class IndexModel : PageModel
{
    private readonly UserRepository _userRepository = new();
    private readonly IJwtGenerationService _jwtService;

    [BindProperty]
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
    public required string Username { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
    public required string Password { get; set; }

    public string Message { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    private Regex _whitelistPattern = new Regex(@"^[a-zA-Z0-9!@#$%^&*.,_]+$");

    public IndexModel(IJwtGenerationService jwtService)
    {
        _jwtService = jwtService;
    }


    // Handle Login
    public IActionResult OnPostLogin()
    {
        if (!ModelState.IsValid)
        {
            Message = "Please correct the errors and try again.";
            return Unauthorized();
        }
        // Validate Username against whitelist
        if (!_whitelistPattern.IsMatch(Username))
        {
            ModelState.AddModelError("Error", "Username contains invalid characters.");
            Message = "Please correct the errors and try again.";
            return Unauthorized();
        }

        bool isValid = _userRepository.VerifyLogin(Username, Password);
        Message = isValid ? "Login successful!" : "Invalid username or password.";

        var role = _userRepository.GetUserRole(Username);

        var token = _jwtService.GenerateToken(Username, role);
        return new JsonResult(new { token });
    }
}