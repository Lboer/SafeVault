using System.Security.Cryptography;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        // Generate a random salt
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        // Derive key using PBKDF2 static method
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100000, // iterations
            HashAlgorithmName.SHA256,
            32 // key length
        );

        // Combine salt + hash for storage
        byte[] hashBytes = new byte[salt.Length + hash.Length];
        Buffer.BlockCopy(salt, 0, hashBytes, 0, salt.Length);
        Buffer.BlockCopy(hash, 0, hashBytes, salt.Length, hash.Length);

        return Convert.ToBase64String(hashBytes);
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        byte[] hashBytes = Convert.FromBase64String(storedHash);

        // Extract salt
        byte[] salt = new byte[16];
        Buffer.BlockCopy(hashBytes, 0, salt, 0, salt.Length);

        // Extract stored hash
        byte[] storedSubkey = new byte[32];
        Buffer.BlockCopy(hashBytes, salt.Length, storedSubkey, 0, storedSubkey.Length);

        // Compute hash for input password
        byte[] computedSubkey = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100000,
            HashAlgorithmName.SHA256,
            32
        );

        // Compare securely
        return CryptographicOperations.FixedTimeEquals(storedSubkey, computedSubkey);
    }
}