using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace winfenixApi.WebApi
{
    public static class EncryptionHelper
    {
        private static readonly string Key = "your-encryption-key";

        public static string Encrypt(string plainText)
        {
            using (var aes = Aes.Create())
            {
                var key = Encoding.UTF8.GetBytes(Key);
                using (var encryptor = aes.CreateEncryptor(key, aes.IV))
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (var sw = new StreamWriter(cs))
                            {
                                sw.Write(plainText);
                            }
                        }
                        var iv = aes.IV;
                        var encrypted = ms.ToArray();
                        var result = new byte[iv.Length + encrypted.Length];
                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(encrypted, 0, result, iv.Length, encrypted.Length);
                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            var fullCipher = Convert.FromBase64String(cipherText);
            using (var aes = Aes.Create())
            {
                var iv = new byte[aes.BlockSize / 8];
                var cipher = new byte[fullCipher.Length - iv.Length];
                Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
                Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);
                var key = Encoding.UTF8.GetBytes(Key);
                using (var decryptor = aes.CreateDecryptor(key, iv))
                {
                    using (var ms = new MemoryStream(cipher))
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (var sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }
    }
}
