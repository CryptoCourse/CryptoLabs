using System;
using System.Security.Cryptography;

namespace CryptoLabsService.Helpers
{
    public class DeterministicCryptoRandomGenerator : RandomNumberGenerator
    {
        private readonly RandomNumberGenerator trueRng;

        private readonly Aes aes;

        private readonly bool useEntropy;

        public long Currentcounter { get; private set; }

        public DeterministicCryptoRandomGenerator(byte[] seed, bool useEntropy)
        {
            this.useEntropy = useEntropy;
            if (useEntropy)
            {
                this.trueRng = RandomNumberGenerator.Create();
            }
            else
            {
                this.aes = Aes.Create();
                if (this.aes == null)
                {
                    throw new NullReferenceException(nameof(this.aes));
                }

                if (seed.Length != this.aes.KeySize / 8)
                {
                    throw new InvalidOperationException();
                }
                this.aes.Mode = CipherMode.ECB;
                this.aes.Key = seed;
            }
        }

        public override void GetBytes(byte[] data, int offset, int count)
        {
            if (this.useEntropy)
            {
                this.trueRng.GetBytes(data, offset, count);
                return;
            }

            if (data.Length - offset < count)
            {
                throw new InvalidOperationException();
            }

            var iterationCount = (count / (this.aes.BlockSize / 8)) + 1;

            using (var aesEncryptor = this.aes.CreateEncryptor())
            {
                long i;
                for (i = this.Currentcounter; i < iterationCount + this.Currentcounter; i++)
                {
                    byte[] encrypted = new byte[this.aes.BlockSize / 8];
                    byte[] rawBlock = new byte[this.aes.BlockSize / 8];
                    byte[] encodedCounter = BitConverter.GetBytes(i);
                    Array.Copy(encodedCounter, rawBlock, encodedCounter.Length);
                    // Transform one block
                    aesEncryptor.TransformBlock(rawBlock, 0, this.aes.BlockSize / 8, encrypted, 0);

                    if (count - (i + 1 - this.Currentcounter) * this.aes.BlockSize / 8 > 0)
                    {
                        Array.Copy(
                            encrypted,
                            0,
                            data,
                            (i - this.Currentcounter) * this.aes.BlockSize / 8 + offset,
                            this.aes.BlockSize / 8);
                    }
                    else
                    {
                        Array.Copy(
                            encrypted,
                            0,
                            data,
                            (i - this.Currentcounter) * this.aes.BlockSize / 8 + offset,
                            count - (i - this.Currentcounter) * this.aes.BlockSize / 8);
                    }
                }

                this.Currentcounter += i;
            }
        }

        public override void GetBytes(byte[] data)
        {
            this.GetBytes(data, 0, data.Length);
        }
        public override void GetNonZeroBytes(byte[] data)
        {
            throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing)
        {
            this.aes?.Dispose();
            this.trueRng?.Dispose();
            base.Dispose(disposing);
        }
    }
}
