namespace CryptoLabsService.Crypto
{
    // The MIT License (MIT)

    // Copyright (c) 2014 Hans Wolff

    // Permission is hereby granted, free of charge, to any person obtaining a copy
    // of this software and associated documentation files (the "Software"), to deal
    // in the Software without restriction, including without limitation the rights
    // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    // copies of the Software, and to permit persons to whom the Software is
    // furnished to do so, subject to the following conditions:

    // The above copyright notice and this permission notice shall be included in
    // all copies or substantial portions of the Software.

    // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    // THE SOFTWARE.

    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;

    public class Aes128CounterMode : SymmetricAlgorithm
    {
        private const int AesBlockSize = 16;

        private readonly Aes aes;

        private readonly byte[] counter;

        public Aes128CounterMode(byte[] counter)
        {
            if (counter == null)
            {
                throw new ArgumentNullException("counter");
            }

            if (counter.Length != AesBlockSize)
            {
                throw new ArgumentException(
                    string.Format(
                        "Counter size must be same as block size (actual: {0}, expected: {1})",
                        counter.Length,
                        AesBlockSize));
            }

            this.aes = Aes.Create();
            this.aes.Mode = CipherMode.ECB;
            this.aes.Padding = PaddingMode.None;

            this.counter = counter;
        }

        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] ignoredParameter)
        {
            return new CounterModeCryptoTransform(this.aes, rgbKey, this.counter);
        }

        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] ignoredParameter)
        {
            return new CounterModeCryptoTransform(this.aes, rgbKey, this.counter);
        }

        public override void GenerateKey()
        {
            this.aes.GenerateKey();
        }

        public override void GenerateIV()
        {
            // IV not needed in Counter Mode
        }

        public override int KeySize => 128;

        public override byte[] Key { get => this.aes.Key; set => this.aes.Key = value; }
    }

    public class CounterModeCryptoTransform : ICryptoTransform
    {
        private readonly byte[] counter;

        private readonly ICryptoTransform counterEncryptor;

        private readonly SymmetricAlgorithm symmetricAlgorithm;

        private readonly Queue<byte> xorMask = new Queue<byte>();

        public int InputBlockSize => this.symmetricAlgorithm.BlockSize / 8;

        public int OutputBlockSize => this.symmetricAlgorithm.BlockSize / 8;

        public bool CanTransformMultipleBlocks => true;

        public bool CanReuseTransform => false;

        public CounterModeCryptoTransform(SymmetricAlgorithm symmetricAlgorithm, byte[] key, byte[] counter)
        {
            if (symmetricAlgorithm == null)
            {
                throw new ArgumentNullException("symmetricAlgorithm");
            }

            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (counter == null)
            {
                throw new ArgumentNullException("counter");
            }

            if (counter.Length != symmetricAlgorithm.BlockSize / 8)
            {
                throw new ArgumentException(
                    string.Format(
                        "Counter size must be same as block size (actual: {0}, expected: {1})",
                        counter.Length,
                        symmetricAlgorithm.BlockSize / 8));
            }

            this.symmetricAlgorithm = symmetricAlgorithm;
            this.counter = counter;

            var zeroIv = new byte[this.symmetricAlgorithm.BlockSize / 8];
            this.counterEncryptor = symmetricAlgorithm.CreateEncryptor(key, zeroIv);
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            var output = new byte[inputCount];
            this.TransformBlock(inputBuffer, inputOffset, inputCount, output, 0);
            return output;
        }

        public int TransformBlock(
            byte[] inputBuffer,
            int inputOffset,
            int inputCount,
            byte[] outputBuffer,
            int outputOffset)
        {
            for (var i = 0; i < inputCount; i++)
            {
                if (this.NeedMoreXorMaskBytes())
                {
                    this.EncryptCounterThenIncrement();
                }

                var mask = this.xorMask.Dequeue();
                outputBuffer[outputOffset + i] = (byte)(inputBuffer[inputOffset + i] ^ mask);
            }

            return inputCount;
        }

        public void Dispose()
        {
        }

        private bool NeedMoreXorMaskBytes()
        {
            return this.xorMask.Count == 0;
        }

        private void EncryptCounterThenIncrement()
        {
            var counterModeBlock = new byte[this.symmetricAlgorithm.BlockSize / 8];

            this.counterEncryptor.TransformBlock(this.counter, 0, this.counter.Length, counterModeBlock, 0);
            this.IncrementCounter();

            foreach (var b in counterModeBlock)
            {
                this.xorMask.Enqueue(b);
            }
        }

        private void IncrementCounter()
        {
            for (var i = this.counter.Length - 1; i >= 0; i--)
            {
                if (++this.counter[i] != 0)
                {
                    break;
                }
            }
        }
    }
}