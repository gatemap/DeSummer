using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Desummer.Scripts
{
    class PasswordEncryption
    {
        public enum EncryptionType
        {
            SHA256 = 0, AES128, TripleDES, TypeMax
        }

        public EncryptionType encryptionType;

        #region 단방향 암호화

        public static string SHA256Hash(string pw)
        {
            SHA256 sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(pw));
            StringBuilder sb = new StringBuilder();

            foreach (byte b in hash)
                sb.AppendFormat("{0:x2}", b);

            return sb.ToString();
        }

        #endregion

        #region 양방향 암호화

        /// <summary>
        /// AES128로 암호화
        /// </summary>
        /// <param name="id">key값이 되어줄 id</param>
        /// <param name="pw">암호화될 입력된 pw</param>
        /// <returns>암호화처리된 string값</returns>
        public static string EncryptByAES128(string id, string pw)
        {
            UTF8Encoding ue = new UTF8Encoding();
            RijndaelManaged rijndael = new RijndaelManaged();
            rijndael.Padding = PaddingMode.PKCS7;
            rijndael.Mode = CipherMode.CBC;
            rijndael.KeySize = 128;

            // key 및 iv 설정
            byte[] pwdBytes = ue.GetBytes(id);
            byte[] keyBytes = new byte[16];
            byte[] IVBytes = new byte[16];
            int lenK = pwdBytes.Length;
            int lenIV = pwdBytes.Length;
            if (lenK > keyBytes.Length) { lenK = keyBytes.Length; }
            if (lenIV > IVBytes.Length) { lenIV = IVBytes.Length; }
            Array.Copy(pwdBytes, keyBytes, lenK);
            Array.Copy(pwdBytes, IVBytes, lenIV);
            rijndael.Key = keyBytes;
            rijndael.IV = IVBytes;

            byte[] message = ue.GetBytes(pw);
            ICryptoTransform transform = rijndael.CreateEncryptor();
            // 암호화 수행 
            byte[] cipherBytes = transform.TransformFinalBlock(message, 0, message.Length);
            rijndael.Clear();

            // 16진수로 변환
            string hex = "";
            foreach (byte x in cipherBytes)
                hex += x.ToString("x2");

            return hex;
        }

        /// <summary>
        /// AES128 복호화
        /// </summary>
        /// <param name="pw">입력한 패스워드</param>
        /// <param name="id">key값으로 입력될 아이디</param>
        /// <returns>key값에 의한 패스워드 복호화된 string값</returns>
        public static string DecrptByAES128(string pw, string id)
        {
            // 사전 설정
            UTF8Encoding ue = new UTF8Encoding();
            RijndaelManaged rijndael = new RijndaelManaged();
            rijndael.Padding = PaddingMode.PKCS7;
            rijndael.Mode = CipherMode.CBC;
            rijndael.KeySize = 128;

            // 16진수 문자열을 바이트 배열로 변환
            byte[] cipherBytes = new byte[pw.Length / 2];

            for (int i = 0; i < cipherBytes.Length; i++)
                cipherBytes[i] = Convert.ToByte(pw.Substring(i * 2, 2), 16);

            // key 및 iv 설정
            byte[] pwdBytes = ue.GetBytes(id);
            byte[] keyBytes = new byte[16];
            byte[] IVBytes = new byte[16];
            int lenK = pwdBytes.Length;
            int lenIV = pwdBytes.Length;
            if (lenK > keyBytes.Length) { lenK = keyBytes.Length; }
            if (lenIV > IVBytes.Length) { lenIV = IVBytes.Length; }
            Array.Copy(pwdBytes, keyBytes, lenK);
            Array.Copy(pwdBytes, IVBytes, lenIV);
            rijndael.Key = keyBytes;
            rijndael.IV = IVBytes;

            ICryptoTransform transform = rijndael.CreateDecryptor();

            // 암호화 수행
            byte[] message = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            rijndael.Clear();

            return Encoding.UTF8.GetString(message);
        }

        /// <summary>
        /// TripleDES 알고리즘으로 암호화
        /// </summary>
        /// <param name="pw">입력한 비밀번호</param>
        /// <param name="id">비밀번호 암호화에 사용될 사용자 id(key값)</param>
        /// <returns>암호화된 string값</returns>
        public static string TripleDESEncrypt(string pw, string id)
        {
            TripleDESCryptoServiceProvider Tripledes = new TripleDESCryptoServiceProvider();

            Tripledes.Mode = CipherMode.ECB;
            Tripledes.Padding = PaddingMode.PKCS7;

            byte[] b_input = Encoding.UTF8.GetBytes(pw);
            byte[] b_key = Encoding.UTF8.GetBytes(id);

            MemoryStream tempStream = new MemoryStream();
            CryptoStream encStream = new CryptoStream(tempStream, Tripledes.CreateEncryptor(b_key, b_key), CryptoStreamMode.Write);

            encStream.Write(b_input, 0, b_input.Length);
            encStream.Close();

            return Convert.ToBase64String(tempStream.ToArray());
        }

        /// <summary>
        /// TripleDES 복호화
        /// </summary>
        /// <param name="pw">저장된 암호화된 비밀번호</param>
        /// <param name="id">key값으로 사용된 사용자 id</param>
        /// <returns>복호화해서 돌려줌</returns>
        public static string TripleDESDecrypt(string pw, string id) 
        {
            TripleDESCryptoServiceProvider Tripledes = new TripleDESCryptoServiceProvider();
            Tripledes.Mode = CipherMode.ECB;
            Tripledes.Padding = PaddingMode.PKCS7;
            byte[] b_input = Convert.FromBase64String(pw);
            byte[] b_key = Encoding.UTF8.GetBytes(id);
            MemoryStream tempStream = new MemoryStream();

            CryptoStream encStream = new CryptoStream(tempStream, Tripledes.CreateDecryptor(b_key, b_key), CryptoStreamMode.Write);
            encStream.Write(b_input, 0, b_input.Length);
            encStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(tempStream.ToArray());
        }

        private string password = "goldapple";
        private string encodedString = "";

        void RSAEncrypt()
        {
            // 암호화 개체 생성
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            // 개인키 생성(복호화용)
            RSAParameters privateKey = RSA.Create().ExportParameters(true);
            rsa.ImportParameters(privateKey);
            string privateKeyText = rsa.ToXmlString(true);

            // 공개키 생성(암호화용)
            RSAParameters publicKey = new RSAParameters();
            publicKey.Modulus = privateKey.Modulus;
            publicKey.Exponent = privateKey.Exponent;
            rsa.ImportParameters(publicKey);
            string publicKeyText = rsa.ToXmlString(false);

            encodedString = RSAEncrypt(password, publicKeyText);
            string decodedString = RSADecrypt(encodedString, privateKeyText);
        }

        // RSA 암호화
        public string RSAEncrypt(string getValue, string pubKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(pubKey);

            //암호화할 문자열을 UFT8인코딩
            byte[] inbuf = (new UTF8Encoding()).GetBytes(getValue);

            //암호화
            byte[] encbuf = rsa.Encrypt(inbuf, false);

            //암호화된 문자열 Base64인코딩
            return Convert.ToBase64String(encbuf);
        }

        // RSA 복호화
        public static string RSADecrypt(string getValue, string priKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(priKey);

            //sValue문자열을 바이트배열로 변환
            byte[] srcbuf = Convert.FromBase64String(getValue);

            //바이트배열 복호화
            byte[] decbuf = rsa.Decrypt(srcbuf, false);

            //복호화 바이트배열을 문자열로 변환
            string sDec = (new UTF8Encoding()).GetString(decbuf, 0, decbuf.Length);
            return sDec;
        }


        #endregion
    }
}
