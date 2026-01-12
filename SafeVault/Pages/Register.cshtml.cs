using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SafeVault.Pages;

public class RegisterModel : PageModel
{
    private readonly UserRepository _userRepository = new();

    [BindProperty]
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
    public required string Username { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string? Email { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
    public required string Password { get; set; }

    public string Message { get; set; } = string.Empty;
    private Regex _whitelistPattern = new Regex(@"^[a-zA-Z0-9!@#$%^&*.,_]+$");

    public void OnGet()
    {
    }

     // Handle Registration
    public void OnPostRegister()
    {
        if (!ModelState.IsValid || !string.IsNullOrWhiteSpace(Email))
        {
            Message = "Please correct the errors and try again.";
            return;
        }
        if (!new EmailAddressAttribute().IsValid(Email))
        {
            Message = "Invalid email format.";
            return;
        }

        var emailLocalPart = Email.Split('@')[0];
        // Validate Username against whitelist
        if (!_whitelistPattern.IsMatch(Username) || !_whitelistPattern.IsMatch(emailLocalPart))
        {
            ModelState.AddModelError("Error", "Username or Email contains invalid characters.");
            Message = "Please correct the errors and try again.";
            return;
        }

        // Register user securely
        _userRepository.RegisterUser(Username, Email, Password);
        Message = "Registration successful!";
    }
}