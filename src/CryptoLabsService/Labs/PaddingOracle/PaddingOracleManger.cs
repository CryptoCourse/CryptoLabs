using System;
using System.Linq;
using System.Security.Cryptography;
using CryptoLabsService.Helpers;

namespace CryptoLabsService.Labs.PaddingOracle
{
    public class PaddingOracleManger
    {
        private const int AesKeySize = 128;
        private const int HmacKeyLen = 32;

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
                    var ivBytes = new byte[aesAlg.KeySize / 8];
                    rand.GetBytes(ivBytes, 0, aesAlg.BlockSize / 8);

                    aesAlg.Key = keyBytes;
                    aesAlg.IV = ivBytes;

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

        public byte[] DecryptCbc(byte[] ciphertext, byte[] seed, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            using (var rand = new DeterministicCryptoRandomGenerator(seed, false))
            {
                byte[] iv;
                using (var aesAlg = Aes.Create())
                {
                    if (ciphertext.Length < aesAlg.BlockSize * 2 / 8)
                    {
                        throw new Exception("Invalid CT len, expecting at least 2 blocks");
                    }
                    aesAlg.KeySize = AesKeySize;
                    var keyBytes = new byte[aesAlg.KeySize / 8];
                    rand.GetBytes(keyBytes, 0, aesAlg.KeySize / 8);
                    iv = ciphertext.Take(aesAlg.BlockSize / 8).ToArray();
                    ciphertext = ciphertext.Skip(aesAlg.BlockSize / 8).ToArray();

                    aesAlg.Key = keyBytes;

                    aesAlg.IV = iv;
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = paddingMode;

                    // Create the streams used for encryption.
                    // Open a new memory stream to write the encrypted data to
                    // Create a crypto stream to perform encryption
                    using (var decryptor = aesAlg.CreateDecryptor())
                    {
                        // write encrypted bytes to memory
                        return TransformHelper.PerformCryptography(decryptor, ciphertext);
                    }
                }
            }
        }

        public byte[] ApplyMac(byte[] data, byte[] seed)
        {
            using (var rand = new DeterministicCryptoRandomGenerator(seed, false))
            {
                var hmacKey = new byte[HmacKeyLen];
                rand.GetBytes(hmacKey, 0, hmacKey.Length);
                using (HMAC hmac = new HMACSHA256(hmacKey))
                {
                    var mac = hmac.ComputeHash(data);
                    var result = new byte[data.Length + hmac.HashSize / 8];
                    Array.Copy(data, 0, result, 0, data.Length);
                    Array.Copy(mac, 0, result, data.Length, hmac.HashSize / 8);
                    return result;
                }
            }
        }

        public byte[] VerifyMac(byte[] data, byte[] seed)
        {
            using (var rand = new DeterministicCryptoRandomGenerator(seed, false))
            {
                var hmacKey = new byte[HmacKeyLen];
                rand.GetBytes(hmacKey, 0, hmacKey.Length);
                using (HMAC hmac = new HMACSHA256(hmacKey))
                {
                    var dataToCheck = data.Take(data.Length - hmac.HashSize / 8).ToArray();
                    var mac = data.Skip(dataToCheck.Length).ToArray();
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