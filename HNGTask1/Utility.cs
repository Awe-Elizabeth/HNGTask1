using System.Security.Cryptography;
using System.Text;

namespace HNGTask1
{
    public static class Utility
    {
        public static (string verifier, string challenge) GeneratePKCE()
        {
            var verifier = GenerateCodeVerifier();
            var challenge = GenerateCodeChallenge(verifier);

            return (verifier, challenge);
        }

        private static string GenerateCodeVerifier()
        {
            var bytes = RandomNumberGenerator.GetBytes(32); // 32 bytes = 43+ chars
            return Base64UrlEncode(bytes);
        }

        private static string GenerateCodeChallenge(string verifier)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(verifier));
            return Base64UrlEncode(hash);
        }

        private static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
    }
}
