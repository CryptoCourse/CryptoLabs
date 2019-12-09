using System;
using System.Runtime.CompilerServices;

namespace CryptoLabsService.Helpers
{
    public static class CompareHelper
    {
        /// <summary>
        /// based on CryptographicOperations.FixedTimeEquals
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool CompareArrays(byte[] first, byte[] second)
        {
            // NoOptimization because we want this method to be exactly as non-short-circuiting
            // as written.
            //
            // NoInlining because the NoOptimization would get lost if the method got inlined.

            if (first.Length != second.Length)
            {
                return false;
            }

            int length = first.Length;
            byte result = 0;

            for (int i = 0; i < length; i++)
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
