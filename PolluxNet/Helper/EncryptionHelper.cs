using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Pollux.Helper
{
    internal static class EncryptionHelper
    {
        private const char Padding = char.MinValue;

        public static string Decrypt(byte[] data)
        {
            byte[] output = Transform(data, false);
            return Encoding.UTF8.GetString(output).TrimEnd(Padding);
        }

        public static byte[] Encrypt(string data)
        {
            data += string.Concat(Enumerable.Repeat(Padding, data.Length - data.Length / 16 * 16));
            byte[] input = Encoding.UTF8.GetBytes(data);
            return Transform(input, true);
        }

        private static byte[] Transform(byte[] input, bool isToEncrypt)
        {
            using (var aes = Aes.Create())
            {
                byte[] data = Encoding.ASCII.GetBytes("452F5057CE6084D352A8AE268CEF83DD");
                byte[] salt = new byte[]
                {
                    57,
                    48,
                    0,
                    0,
                    49,
                    212,
                    0,
                    0,
                };
                byte[] key, iv;
                DeriveKeyAndIV(data, salt, 5, out key, out iv);
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.Zeros;
                using (var memoryStream = new MemoryStream())
                using (var cryptoTransform = isToEncrypt
                    ? aes.CreateEncryptor() : aes.CreateDecryptor())
                using (var cryptoStream = new CryptoStream(memoryStream,
                    cryptoTransform, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(input, 0, input.Length);
                    return memoryStream.ToArray();
                }
            }
        }

        private static void DeriveKeyAndIV(byte[] data, byte[] salt, int count, out byte[] key, out byte[] iv)
        {
            data = data.Concat(salt ?? new byte[0]).ToArray();

            var list = new List<byte>();

            using (var sha = SHA1.Create())
            {
                do
                {
                    byte[] array2 = list.Concat(data).ToArray();
                    byte[] array = sha.ComputeHash(array2);

                    for (int i = 1; i < count; ++i)
                    {
                        array = sha.ComputeHash(array);
                    }

                    list.AddRange(array);
                }
                while (list.Count < 48);
            }

            key = new byte[32];
            iv = new byte[16];
            list.CopyTo(0, key, 0, 32);
            list.CopyTo(32, iv, 0, 16);
        }
    }

    public static class AesEncryption
    {
        private static readonly byte[] key = Convert.FromBase64String("KJEYEv6jlvipZeccL0FzS9xXNDaD1XkFpgp/zERYF7E=");
        private static readonly byte[] iv = Convert.FromBase64String("jlE5Y3nXvBouTvunlBmeTw==");

        public static string Encrypt(string encryptData)
        {
            var bytes = Encoding.UTF8.GetBytes(encryptData);
            using (var aes = new AesCryptoServiceProvider())
            {
                using (var ms = new MemoryStream())
                using (var encryptor = aes.CreateEncryptor(key, iv))
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(bytes, 0, bytes.Length);
                    cs.FlushFinalBlock();
                    var cipher = ms.ToArray();
                    return Convert.ToBase64String(cipher);
                }
            }
        }

        public static string Decrypt(string decryptData)
        {
            var bytes = Convert.FromBase64String(decryptData);
            using (var aes = new AesCryptoServiceProvider())
            {
                using (var ms = new MemoryStream())
                using (var decryptor = aes.CreateDecryptor(key, iv))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    cs.Write(bytes, 0, bytes.Length);
                    cs.FlushFinalBlock();
                    var cipher = ms.ToArray();
                    return Encoding.UTF8.GetString(cipher);
                }
            }
        }
    }


    //https://github.com/ASP-NET-MVC/aspnetwebstack/blob/4e40cdef9c8a8226685f95ef03b746bc8322aa92/src/System.Web.Helpers/Crypto.cs
    public static class Crypto
    {
        private const int PBKDF2IterCount = 1000; // default for Rfc2898DeriveBytes
        private const int PBKDF2SubkeyLength = 256 / 8; // 256 bits
        private const int SaltSize = 128 / 8; // 128 bits

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "It really is a byte length")]
        internal static byte[] GenerateSaltInternal(int byteLength = SaltSize)
        {
            byte[] buf = new byte[byteLength];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(buf);
            }
            return buf;
        }

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "It really is a byte length")]
        public static string GenerateSalt(int byteLength = SaltSize)
        {
            return Convert.ToBase64String(GenerateSaltInternal(byteLength));
        }

        public static string Hash(string input, string algorithm = "sha256")
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            return Hash(Encoding.UTF8.GetBytes(input), algorithm);
        }

        public static string Hash(byte[] input, string algorithm = "sha256")
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            using (HashAlgorithm alg = HashAlgorithm.Create(algorithm))
            {
                if (alg != null)
                {
                    byte[] hashData = alg.ComputeHash(input);
                    return BinaryToHex(hashData);
                }
                else
                {
                    throw new NotSupportedException(string.Format("{0} is not supported!", algorithm));
                }
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SHA", Justification = "Consistent with the Framework, which uses SHA")]
        public static string SHA1(string input)
        {
            return Hash(input, "sha1");
        }

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SHA", Justification = "Consistent with the Framework, which uses SHA")]
        public static string SHA256(string input)
        {
            return Hash(input, "sha256");
        }

        /* =======================
         * HASHED PASSWORD FORMATS
         * =======================
         * 
         * Version 0:
         * PBKDF2 with HMAC-SHA1, 128-bit salt, 256-bit subkey, 1000 iterations.
         * (See also: SDL crypto guidelines v5.1, Part III)
         * Format: { 0x00, salt, subkey }
         */

        public static string HashPassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            // Produce a version 0 (see comment above) password hash.
            byte[] salt;
            byte[] subkey;
            using (var deriveBytes = new Rfc2898DeriveBytes(password, SaltSize, PBKDF2IterCount))
            {
                salt = deriveBytes.Salt;
                subkey = deriveBytes.GetBytes(PBKDF2SubkeyLength);
            }

            byte[] outputBytes = new byte[1 + SaltSize + PBKDF2SubkeyLength];
            Buffer.BlockCopy(salt, 0, outputBytes, 1, SaltSize);
            Buffer.BlockCopy(subkey, 0, outputBytes, 1 + SaltSize, PBKDF2SubkeyLength);
            return Convert.ToBase64String(outputBytes);
        }

        // hashedPassword must be of the format of HashWithPassword (salt + Hash(salt+input)
        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            if (hashedPassword == null)
            {
                throw new ArgumentNullException("hashedPassword");
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            byte[] hashedPasswordBytes = Convert.FromBase64String(hashedPassword);

            // Verify a version 0 (see comment above) password hash.

            if (hashedPasswordBytes.Length != (1 + SaltSize + PBKDF2SubkeyLength) || hashedPasswordBytes[0] != 0x00)
            {
                // Wrong length or version header.
                return false;
            }

            byte[] salt = new byte[SaltSize];
            Buffer.BlockCopy(hashedPasswordBytes, 1, salt, 0, SaltSize);
            byte[] storedSubkey = new byte[PBKDF2SubkeyLength];
            Buffer.BlockCopy(hashedPasswordBytes, 1 + SaltSize, storedSubkey, 0, PBKDF2SubkeyLength);

            byte[] generatedSubkey;
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, PBKDF2IterCount))
            {
                generatedSubkey = deriveBytes.GetBytes(PBKDF2SubkeyLength);
            }
            return ByteArraysEqual(storedSubkey, generatedSubkey);
        }

        internal static string BinaryToHex(byte[] data)
        {
            char[] hex = new char[data.Length * 2];

            for (int iter = 0; iter < data.Length; iter++)
            {
                byte hexChar = ((byte)(data[iter] >> 4));
                hex[iter * 2] = (char)(hexChar > 9 ? hexChar + 0x37 : hexChar + 0x30);
                hexChar = ((byte)(data[iter] & 0xF));
                hex[(iter * 2) + 1] = (char)(hexChar > 9 ? hexChar + 0x37 : hexChar + 0x30);
            }
            return new string(hex);
        }

        // Compares two byte arrays for equality. The method is specifically written so that the loop is not optimized.
        [MethodImpl(MethodImplOptions.NoOptimization)]
        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            bool areSame = true;
            for (int i = 0; i < a.Length; i++)
            {
                areSame &= (a[i] == b[i]);
            }
            return areSame;
        }
    }
}
