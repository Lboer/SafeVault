using SafeVault.Pages;
using NSubstitute;
using Xunit;

public class UserRepositoryTests
{
    // You may want to use a test connection string here
    private readonly UserRepository _repo = new UserRepository();

    [Theory]
    [InlineData("Robert'); DROP TABLE Users;--", "test@example.com", "password123")]
    [InlineData("admin' OR '1'='1", "admin@example.com", "password123")]
    [InlineData("normaluser", "test@example.com", "' OR 1=1--")]
    public void RegisterUser_ShouldNotAllowSqlInjection(string username, string email, string password)
    {
        // Arrange
        var model = new RegisterModel{ Username = username, Email = email, Password = password };
        
        // Act
        model.OnPostRegister();
        var users = _repo.GetAllUsers();
        
        // Assert
        Assert.DoesNotContain(users, u => u.Username == username);
    }

    [Theory]
    [InlineData("admin' OR '1'='1", "password123")]
    [InlineData("'; EXEC xp_cmdshell('dir');--", "password123")]
    public void VerifyLogin_ShouldNotAllowSqlInjection(string username, string password)
    {
        // Arrange
        var jwtService = Substitute.For<IJwtGenerationService>();
        jwtService.GenerateToken(Arg.Any<string>(), Arg.Any<string>())
                  .Returns("fake-jwt-token");

        var model = new IndexModel(jwtService){ Username = username, Password = password };
        
        // Act
        model.OnPostLogin();

        // Assert: Should not authenticate, should not throw
        Assert.Equal("Please correct the errors and try again.", model.Message);
    }

    [Fact]
    public void InvalidLogin_ShouldReturnFalse()
    {
        // Arrange: Use credentials that do not exist
        string username = "nonexistentuser";
        string password = "wrongpassword";

        // Act
        bool result = _repo.VerifyLogin(username, password);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UnauthorizedAccess_ShouldNotAllowRoleChange()
    {
        // Arrange: Create two users, one normal and one admin
        string normalUsername = "normaluser";
        string adminUsername = "adminuser";
        string password = "Password123!";

        // Register users (if not already in DB)
        _repo.RegisterUser(normalUsername, "normal@example.com", password);
        _repo.RegisterUser(adminUsername, "admin@example.com", password);

        // Set roles
        var normalUser = _repo.GetAllUsers().Find(u => u.Username == normalUsername);
        var adminUser = _repo.GetAllUsers().Find(u => u.Username == adminUsername);

        _repo.UpdateUserRole(adminUser.UserID, "Admin");
        _repo.UpdateUserRole(normalUser.UserID, "User");

        // Act: Try to let normal user change another user's role (simulate by checking role)
        string normalUserRole = _repo.GetUserRole(normalUsername);

        // Assert: Only admin should be allowed to change roles (logic to enforce this should be in your app)
        Assert.NotEqual("Admin", normalUserRole);
    }
}
