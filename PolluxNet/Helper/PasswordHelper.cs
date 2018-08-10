using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Pollux.Helper
{
    public class PasswordHelper
    {
        public string Encrypt(string password)
        {
            var passBytes = Encoding.Default.GetBytes(password);
            var encryptBytes = ProtectedData.Protect(passBytes, null, DataProtectionScope.CurrentUser);

            return Encoding.Default.GetString(encryptBytes);
        }
        public string Decrypt(string encryptText)
        {
            var encryptBytes = Encoding.Default.GetBytes(encryptText);
            var passBytes = ProtectedData.Unprotect(encryptBytes, null, DataProtectionScope.CurrentUser);

            return Encoding.Default.GetString(passBytes);
        }
    }
}
