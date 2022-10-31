using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;

namespace WalletSolution.Helpers
{
    public static class HasherHelper
    {
        public static string Hasher(string text)
        {
            using (var sha256 = SHA256.Create())
            {
                 
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
               
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
