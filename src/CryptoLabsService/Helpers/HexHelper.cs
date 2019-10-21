using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoLabsService.Helpers
{
    public class HexHelper
    {
        public static string HexFromByteArray(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
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
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }
}
