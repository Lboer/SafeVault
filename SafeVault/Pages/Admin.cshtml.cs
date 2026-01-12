
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

[Authorize(Roles = "Admin")]
public class AdminModel : PageModel
{
    private readonly UserRepository _userRepository = new();

    public List<User> Users { get; set; } = new();

    public void OnGet()
    {
        Users = _userRepository.GetAllUsers();
    }

    public IActionResult OnPostChangeRole(int userId, string newRole)
    {
        _userRepository.UpdateUserRole(userId, newRole);
        return RedirectToPage();
    }
}
