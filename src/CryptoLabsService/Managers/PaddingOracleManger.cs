using CryptoLabsService.Helpers;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace CryptoLabsService.Managers
{
    public class PaddingOracleManger
    {
        private const int AesKeySize = 128;

        public byte[] EncryptCbc(byte[] data, byte[] seed, bool includeIv = true)
        {
            using (var rand = new DeterministicCryptoRandomGenerator(seed, false))
            {
                byte[] ciphertext;
                using (var aesAlg = Aes.Create())
                {
                    aesAlg.KeySize = AesKeySize;
                    var keyBytes = new byte[aesAlg.KeySize / 8];
                    rand.GetBytes(keyBytes, 0, aesAlg.KeySize / 8);

                    rand.GetBytes(aesAlg.IV, 0, aesAlg.BlockSize / 8);

                    aesAlg.Key = keyBytes;
                    aesAlg.Padding = PaddingMode.None;

                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = PaddingMode.PKCS7;

                    // Create the streams used for encryption. 
                    // Open a new memory stream to write the encrypted data to
                    // Create a crypto stream to perform encryption
                    using (var ecryptor = aesAlg.CreateEncryptor())
                    {
                        // write encrypted bytes to memory
                        ciphertext = TransformHelper.PerformCryptography(ecryptor, data);
                    }
                    if (!includeIv)
                    {
                        return ciphertext;
                    }

                    var result = new byte[aesAlg.IV.Length + ciphertext.Length];
                    Array.Copy(aesAlg.IV, 0, result, 0, aesAlg.IV.Length);
                    Array.Copy(ciphertext, 0, result, aesAlg.IV.Length, ciphertext.Length);

                    // Return the encrypted bytes from the memory stream. 
                    return result;
                }
            }
        }

        public byte[] DecryptCbc(byte[] ciphertext, byte[] seed)
        {
            using (var rand = new DeterministicCryptoRandomGenerator(seed, false))
            {
                byte[] iv;
                using (var aesAlg = Aes.Create())
                {
                    aesAlg.KeySize = AesKeySize;
                    var keyBytes = new byte[aesAlg.KeySize / 8];
                    rand.GetBytes(keyBytes, 0, aesAlg.KeySize / 8);
                    iv = ciphertext.Take(aesAlg.BlockSize).ToArray();
                    ciphertext = ciphertext.Skip(aesAlg.BlockSize).ToArray();

                    aesAlg.Key = keyBytes;

                    aesAlg.IV = iv;
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = PaddingMode.PKCS7;

                    // Create the streams used for encryption. 
                    // Open a new memory stream to write the encrypted data to
                    // Create a crypto stream to perform encryption
                    using (var ecryptor = aesAlg.CreateDecryptor())
                    {
                        // write encrypted bytes to memory
                        return TransformHelper.PerformCryptography(ecryptor, ciphertext);
                    }
                }
            }
        }

        public byte[] ApplyMac(byte[] data, byte[] seed)
        {
            using (var rand = new DeterministicCryptoRandomGenerator(seed, false))
            {
                using (HMAC hmac = HMAC.Create())
                {
                    rand.GetBytes(hmac.Key, 0, hmac.Key.Length);
                    var mac = hmac.ComputeHash(data);
                    var result = new byte[data.Length + hmac.HashSize];
                    Array.Copy(data, 0, result, 0, data.Length);
                    Array.Copy(mac, 0, result, data.Length, hmac.HashSize);
                    return result;
                }
            }
        }

        public byte[] VerifyMac(byte[] data, byte[] seed)
        {
            using (var rand = new DeterministicCryptoRandomGenerator(seed, false))
            {
                using (HMAC hmac = HMAC.Create())
                {
                    rand.GetBytes(hmac.Key, 0, hmac.Key.Length);

                    var mac = data.Skip(hmac.HashSize).ToArray();
                    var dataToCheck = data.Take(data.Length - hmac.HashSize).ToArray();
                    var computedMac = hmac.ComputeHash(dataToCheck);
                    if (!CompareHelper.CompareArrays(mac, computedMac))
                    {
                        throw new Exception("Invalid MAC!");
                    }

                    return dataToCheck;
                }
            }
        }
    }
}
