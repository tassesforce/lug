using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Linq;

namespace lug.String.Encrypt
{
    ///<summary>Helper d'encryption/decryption de string. Merci a CraigTP pour le partage</summary>
    public class StringEncryptHelper
    {
        /// <summary>Encryption d'une chaine de caracteres</summary>
        /// <param name="criterias">Criteres d'encryption de la chaine de caracteres</param>
        /// <returns>string : la chaine encryptee</returns>
        public static string Encrypt(StringEncryptCriteria criterias)
        {
            if (!string.IsNullOrEmpty(criterias.Text))
            {
                var saltStringBytes = GenerateBitsOfRandomEntropy(criterias.KeySize);
                var ivStringBytes = GenerateBitsOfRandomEntropy(criterias.KeySize);
                var plainTextBytes = Encoding.UTF8.GetBytes(criterias.Text);
                using (var password = new Rfc2898DeriveBytes(Encoding.ASCII.GetBytes(criterias.PassPhrase), saltStringBytes, criterias.DerivationIterations))
                {
                    var keyBytes = password.GetBytes(criterias.KeySize / 8);
                    using (var symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.BlockSize = criterias.KeySize;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;
                        using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                        using (var memoryStream = new MemoryStream())
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            var cipherTextBytes = saltStringBytes;
                            cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                            cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                            memoryStream.Close();
                            cryptoStream.Close();
                            return Convert.ToBase64String(cipherTextBytes);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>Decryption de chaine de caracteres</summary>
        /// <param name="criterias">criteres de decryption</param>
        /// <returns>string : la chaine de caracteres decryptee</returns>
        public static string Decrypt(StringEncryptCriteria criterias)
        {
            if (!string.IsNullOrEmpty(criterias.Text))
            {
                // Get the complete stream of bytes that represent:
                // [x bytes of Salt] + [y bytes of IV] + [n bytes of CipherText]
                var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(criterias.Text);
                // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
                var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(criterias.KeySize / 8).ToArray();
                // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
                var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(criterias.KeySize / 8).Take(criterias.KeySize / 8).ToArray();
                // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
                var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((criterias.KeySize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((criterias.KeySize / 8) * 2)).ToArray();

                using (var password = new Rfc2898DeriveBytes(Encoding.ASCII.GetBytes(criterias.PassPhrase), saltStringBytes, criterias.DerivationIterations))
                {
                    var keyBytes = password.GetBytes(criterias.KeySize / 8);
                    using (var symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.BlockSize = criterias.KeySize;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;
                        using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            var plainTextBytes = new byte[cipherTextBytes.Length];
                            var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                            memoryStream.Close();
                            cryptoStream.Close();
                            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>Genere des bits aleatoires</summary>
        private static byte[] GenerateBitsOfRandomEntropy(int keySize)
        {
            var randomBytes = new byte[keySize / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}