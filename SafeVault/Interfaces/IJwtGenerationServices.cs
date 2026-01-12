public interface IJwtGenerationService
{
    public string GenerateToken(string username, string role);
}