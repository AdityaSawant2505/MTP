using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DAMS.Models.Auth
{
    public class PasswordHelper
    {
        public (string hashedPassword, string salt) HashPassword(string password)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var salt = new byte[16]; // 128-bit salt
                rng.GetBytes(salt);

                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA512))
                {
                    var hashedPassword = pbkdf2.GetBytes(64);
                    return (Convert.ToBase64String(hashedPassword), Convert.ToBase64String(salt));
                }
            }
        }
    }
}
