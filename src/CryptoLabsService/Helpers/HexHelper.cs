using System;
using System.Linq;

namespace CryptoLabsService.Helpers
{
    public class HexHelper
    {
        public static string HexFromByteArray(byte[] ba)
        {
            return Convert.ToBase64String(ba);
        }

        private static bool IsHexValid(string source)
        {
            var availableChars = "01234567890abcdef";
            if (source.ToLower().ToCharArray().Where(c => !availableChars.Contains(c)).Count() > 0
                || source.Length % 2 > 0)
            {
                return false;
            }

            return true;
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Convert.FromBase64String(hex);
        }
    }
}