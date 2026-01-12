using SafeVault.Pages;
using NSubstitute;

public class IndexModelXssTests
{
    [Theory]
    [InlineData("<script>alert('xss')</script>")]
    [InlineData("user<script>")]
    [InlineData("user<img src=x onerror=alert(1)>")]
    [InlineData("user\" onmouseover=\"alert(1)")]
    [InlineData("user' OR '1'='1")]
    public void OnPostLogin_ShouldRejectXssUsername(string maliciousUsername)
    {
        // Arrange
        var jwtService = Substitute.For<IJwtGenerationService>();
        jwtService.GenerateToken(Arg.Any<string>(), Arg.Any<string>())
                  .Returns("fake-jwt-token");

        var model = new IndexModel(jwtService){ Username = maliciousUsername, Password = "ValidPassword123"};

        // Act
        model.OnPostLogin();

        // Assert
        Assert.Equal("Please correct the errors and try again.", model.Message);
    }

    [Theory]
    [InlineData("<script>alert('xss')</script>")]
    [InlineData("user<script>")]
    [InlineData("user<img src=x onerror=alert(1)>")]
    [InlineData("user\" onmouseover=\"alert(1)")]
    [InlineData("user' OR '1'='1")]
    public void OnPostRegister_ShouldRejectXssUsername(string maliciousUsername)
    {
        // Arrange
        var model = new RegisterModel{ Username = maliciousUsername, Email = "test@example.com", Password = "ValidPassword123"};
        model.ModelState.SetModelValue("Username", maliciousUsername, maliciousUsername);

        // Act
        model.OnPostRegister();

        // Assert
        Assert.Equal("Please correct the errors and try again.", model.Message);
    }

    [Theory]
    [InlineData("xss<script>@example.com")]
    [InlineData("evil<img>@example.com")]
    [InlineData("bad\"onmouseover=\"alert(1)@example.com")]
    public void OnPostRegister_ShouldRejectXssEmail(string maliciousEmail)
    {
        // Arrange
        var model = new RegisterModel{ Username = "ValidUser", Email = maliciousEmail, Password = "ValidPassword123"};
        model.ModelState.SetModelValue("Email", maliciousEmail, maliciousEmail);

        // Act
        model.OnPostRegister();

        // Assert
        Assert.Equal("Please correct the errors and try again.", model.Message);
    }
}
