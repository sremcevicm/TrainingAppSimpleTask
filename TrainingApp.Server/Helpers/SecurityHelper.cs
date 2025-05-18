using System.Security.Cryptography;
using System.Text;

namespace TrainingApp.Server.Helpers
{
    public static class SecurityHelper
    {
        public static string HashAccessCode(string code)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(code);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
