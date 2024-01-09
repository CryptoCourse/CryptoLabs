using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CryptoLabsService.Labs.EncryptionModeOracle;
using Microsoft.AspNetCore.Mvc;

namespace CryptoLabsService.Labs.EcbDecryption
{
    [ApiController]
    [Route("api/EcbDecryption")]
    public class EcbDecryptionController : Controller
    {
        private const int AesBlockSize = 16;

        private readonly BlockCipherOracleManager blockCipherOracleManager;

        public EcbDecryptionController(BlockCipherOracleManager blockCipherOracleManager)
        {
            this.blockCipherOracleManager = blockCipherOracleManager;
        }

        // GET api/
        [HttpGet]
        public string Get()
        {
            return "operating";
        }

        // [HttpPost]
        // [Route("{userId1}/{challengeId}")]
        // public string Post(
        //     [FromRoute] string userId,
        //     [FromRoute] string challengeId,
        //     [FromBody] string value)
        // {
        //     using (var hash = SHA256.Create())
        //     {
        //         var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));

        //         var userData = Convert.FromBase64String(value);
        //         var targetData = this.GetTargetData(seed);
        //         var paddingData = this.GetPaddingData(seed, targetData);
        //         var dataToEncrypt = paddingData.Concat(userData).Concat(targetData).ToArray();

        //         var result = this.blockCipherOracleManager.EncryptOracle(dataToEncrypt, seed, true, false);
        //         return Convert.ToBase64String(result);
        //     }
        // }

        [HttpPost]
        [Route("{userId}/{challengeId}/noentropy")]
        public string PostNoEntropy(
            [FromRoute] string userId,
            [FromRoute] string challengeId,
            [FromBody] string value)
        {
            using (var hash = SHA256.Create())
            {
                var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));

                var userData = Convert.FromBase64String(value);
                var targetData = this.GetTargetData(seed);
                var paddingData = this.GetPaddingData(seed, targetData);
                var dataToEncrypt = paddingData.Concat(userData).Concat(targetData).ToArray();

                var result = this.blockCipherOracleManager.EncryptOracle(dataToEncrypt, seed, false, false);
                return Convert.ToBase64String(result);
            }
        }

        [HttpGet]
        [Route("{userId}/{challengeId}/verify")]
        public string GetVerify(
            [FromRoute] string userId,
            [FromRoute] string challengeId)
        {
            using (var hash = SHA256.Create())
            {
                var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));
                if (seed[seed.Length - 1] % 2 == 0)
                {
                    var targetData = this.GetTargetData(seed);
                    return Convert.ToBase64String(targetData);
                }

                return "CBC";
            }
        }

        private byte[] GetTargetData(byte[] seed)
        {
            var baseB = new byte[]
            {
                1,2,3,4,5,6
            };
            using (var hash = SHA256.Create())
            {
                return baseB.Concat(this.blockCipherOracleManager.GetRandomString(
                    //16,
                    seed[0] * AesBlockSize + seed[3],
                    hash.ComputeHash(seed),
                    false)).ToArray();
            }
        }

        private byte[] GetPaddingData(byte[] seed, byte[] targetData)
        {
            //return new byte[1]{250};
            using (var hash = SHA256.Create())
            {
                return this.blockCipherOracleManager.GetRandomString(
                    seed[1] * AesBlockSize + seed[4],
                    hash.ComputeHash(targetData),
                    false);
            }
        }
    }
}