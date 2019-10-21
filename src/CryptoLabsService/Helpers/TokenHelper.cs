using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLabsService.Helpers
{
    public static class TokenHelper
    {
        public const int AesBlockSize = 16;

        // need 3 blocks for attack, extra - for fun
        public static int BlockCount { get; set; } = 4;

        public static string AvailableTokenChars { get; set; } = "0123456789abcdefghijklmnopqrstuvwxyz-";

        public static string GetSecretTokenUser(byte[] seed)
        {

            using (var generator = new DeterministicCryptoRandomGenerator(seed, false))
            {
                var bytes = new byte[AesBlockSize * BlockCount];
                generator.GetBytes(bytes);
                var chars = bytes
                    .Select(b => AvailableTokenChars[b % AvailableTokenChars.Length]);
                var token = new string(chars.ToArray());
                return token;
            }
        }

        public static string GetSecretTokenAdmin(byte[] seed)
        {
            using (var hash = SHA256.Create())
            {
                using (var generator = new DeterministicCryptoRandomGenerator(hash.ComputeHash(seed), false))
                {
                    var bytes = new byte[AesBlockSize * BlockCount];
                    generator.GetBytes(bytes);
                    var chars = bytes
                        .Select(b => AvailableTokenChars[b % AvailableTokenChars.Length]);
                    var token = new string(chars.ToArray());
                    return token;
                }
            }
        }

        public static bool ValidateTokenString(string token)
        {
            return token.All(l => AvailableTokenChars.Contains(l));
        }

        public static bool ValidateTokenAdmin(string token, byte[] seed)
        {
            var adminToken = Encoding.ASCII.GetBytes(TokenHelper.GetSecretTokenAdmin(seed));
            var suppliedToken = Encoding.ASCII.GetBytes(token);
            return CompareHelper.CompareArrays(adminToken, suppliedToken);
        }

        public static bool ValidateTokenUser(string token, byte[] seed)
        {
            var userToken = Encoding.ASCII.GetBytes(TokenHelper.GetSecretTokenUser(seed));
            var suppliedToken = Encoding.ASCII.GetBytes(token);
            return CompareHelper.CompareArrays(userToken, suppliedToken);
        }
    }
}
