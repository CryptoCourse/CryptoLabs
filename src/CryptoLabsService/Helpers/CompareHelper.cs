using System;

namespace CryptoLabsService.Helpers
{
    public static class CompareHelper
    {
        public static bool CompareArrays(byte[] first, byte[] second)
        {
            byte result = 0;
            if (first.Length != second.Length)
            {
                return false;
            }

            for (int i = 0; i < first.Length; i++)
            {
                result |= (byte)(first[i] ^ second[i]);
            }

            return result == 0;
        }

        public static bool InsecureCompareArrays(byte[] source, byte[] target, int delay)
        {
            var toSend = BitConverter.ToString(source).Replace("-", "");

            if (source.Length != target.Length)
            {
                return false;
            }

            for (var index = 0; index < target.Length; index++)
            {
                System.Threading.Thread.Sleep(delay);
                if (source[index] != target[index])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
