using MySql.Data.MySqlClient;

public class UserRepository
{
    private readonly string _connectionString = "Server=localhost;Database=safevault;User ID=root;Password=password;";
    //private readonly string _connectionString = "Server=localhost;Database=safevault;User ID=test;Password=test;";

    // Register a new user
    public void RegisterUser(string username, string email, string password)
    {
        string passwordHash = PasswordHasher.HashPassword(password);

        string query = "INSERT INTO Users (Username, Email, PasswordHash) VALUES (@Username, @Email, @PasswordHash)";

        using var connection = new MySqlConnection(_connectionString);
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@Username", username);
        command.Parameters.AddWithValue("@Email", email);
        command.Parameters.AddWithValue("@PasswordHash", passwordHash);

        connection.Open();
        command.ExecuteNonQuery();
    }

    // Verify login credentials
    public bool VerifyLogin(string username, string password)
    {
        string query = "SELECT PasswordHash FROM Users WHERE Username = @Username";

        using var connection = new MySqlConnection(_connectionString);
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@Username", username);

        connection.Open();
        var result = command.ExecuteScalar();

        if (result != null)
        {
            string storedHash = result.ToString();
            return PasswordHasher.VerifyPassword(password, storedHash!);
        }

        return false;
    }

    
    // Get all users
    public List<User> GetAllUsers()
    {
        var users = new List<User>();
        string query = "SELECT UserID, Username, Email, PasswordHash, Roles FROM Users";

        using var connection = new MySqlConnection(_connectionString);
        using var command = new MySqlCommand(query, connection);

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var user = new User
            {
                UserID = reader.GetInt32("UserID"),
                Username = reader.GetString("Username"),
                Email = reader.GetString("Email"),
                Roles = reader.GetString("Roles")
            };
            users.Add(user);
        }
        return users;
    }

    
    // Update a user's role
    public void UpdateUserRole(int userId, string newRole)
    {
        string query = "UPDATE Users SET Roles = @Roles WHERE UserID = @UserID";

        using var connection = new MySqlConnection(_connectionString);
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@Roles", newRole);
        command.Parameters.AddWithValue("@UserID", userId);

        connection.Open();
        command.ExecuteNonQuery();
    }

    // Get role
    public string GetUserRole(string username)
    {
        string query = "SELECT Roles FROM Users WHERE Username = @Username";

        using var connection = new MySqlConnection(_connectionString);
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@Username", username);

        connection.Open();
        return command.ExecuteScalar().ToString();
    }
}