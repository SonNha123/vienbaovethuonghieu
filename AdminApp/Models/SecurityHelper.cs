using System.Security.Cryptography;
using System.Text;

namespace New_folder.Models;

public static class SecurityHelper
{
    public static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(hashedBytes).ToLower();
        }
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        var inputHash = HashPassword(password);
        return string.Equals(inputHash, hashedPassword, StringComparison.OrdinalIgnoreCase);
    }
}
