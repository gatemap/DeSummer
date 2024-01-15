using System.Security.Cryptography;
using System.Text;

namespace Desummer.Scripts
{
    class PasswordEncryption
    {
        public static string SHA256Hash(string pw)
        {
            SHA256 sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(pw));
            StringBuilder sb = new StringBuilder();

            foreach (byte b in hash)
                sb.AppendFormat("{0:x2}", b);

            return sb.ToString();
        }
    }
}
