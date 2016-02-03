using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MondoAspNetMvcSample
{
    public static class CryptoHelper
    {
        public static string GenerateRandomString(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            var sb = new StringBuilder();
            using (var provider = new RNGCryptoServiceProvider())
            {
                while (sb.Length != length)
                {
                    byte[] oneByte = new byte[1];
                    provider.GetBytes(oneByte);
                    char character = (char)oneByte[0];
                    if (valid.Contains(character))
                    {
                        sb.Append(character);
                    }
                }
            }

            return sb.ToString();
        }
    }
}