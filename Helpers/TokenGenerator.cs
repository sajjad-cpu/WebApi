using System.Security.Cryptography;
namespace Pinjet.Helpers
{
    public class TokenGenerator
    {
        public static string GenerateRefreshToken()
        {
            var randomBytes =new byte[32];

            RandomNumberGenerator.Fill(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }
    }
}
