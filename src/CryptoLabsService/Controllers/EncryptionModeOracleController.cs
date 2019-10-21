namespace CryptoLabsService.Controllers
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    using CryptoLabsService.Managers;

    using Microsoft.AspNetCore.Mvc;

    [Route("api/EncryptionModeOracle")]
    public class EncryptionModeOracleController : Controller
    {
        private readonly BlockCipherOracleManager blockCipherOracleManager;

        public EncryptionModeOracleController(BlockCipherOracleManager blockCipherOracleManager)
        {
            this.blockCipherOracleManager = blockCipherOracleManager;
        }

        // GET api/
        [HttpGet]
        public string Get()
        {
            return "operating";
        }

        [HttpPost]
        [Route("{userId}/{challengeId}")]
        public string Post(
            [FromRoute] string userId,
            [FromRoute] string challengeId,
            [FromBody] string value)
        {
            using (var hash = SHA256.Create())
            {
                var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));
                var data = Convert.FromBase64String(value);
                var result = this.blockCipherOracleManager.EncryptOracle(data, seed);
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
                    return "ECB";
                }
                return "CBC";
            }
        }



        [HttpPost]
        [Route("{userId}/{challengeId}/noentropy")]
        public string PostNoEnctropy(
            [FromRoute] string userId,
            [FromRoute] string challengeId,
            [FromBody] string value)
        {
            using (var hash = SHA256.Create())
            {
                var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));
                var data = Convert.FromBase64String(value);
                var result = this.blockCipherOracleManager.EncryptOracle(data, seed, false);
                return Convert.ToBase64String(result);
            }
        }
    }
}
