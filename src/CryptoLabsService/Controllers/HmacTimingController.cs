namespace CryptoLabsService.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using CryptoLabsService.Helpers;
    using CryptoLabsService.Managers;

    using Microsoft.AspNetCore.Mvc;

    [Route("api/HmacTiming")]
    public class HmacTimingController : Controller
    {
        private const int AesBlockSize = 16;

        private readonly BlockCipherOracleManager blockCipherOracleManager;

        public HmacTimingController(BlockCipherOracleManager blockCipherOracleManager)
        {
            this.blockCipherOracleManager = blockCipherOracleManager;
        }

        // GET api/
        [HttpGet]
        public string Get()
        {
            return "operating";
        }

        [HttpGet]
        [Route("{userId}/{challengeId}/data={data}&mac={mac}&delay={delay}")]
        public string GetVerify(
            [FromRoute] string userId,
            [FromRoute] string challengeId,
            [FromRoute] string data,
            [FromRoute] string mac,
            [FromRoute] int delay)
        {
            delay = delay > 1000 ? 1000 : delay;

            using (var hash = SHA256.Create())
            {

                var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));

                var macHash = new HMACSHA256(seed);
                var targetHash = macHash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId + data));
                if (!CompareHelper.InsecureCompareArrays(targetHash.Take(8).ToArray(), HexHelper.StringToByteArray(mac), delay))
                {
                    return "INVALID_MAC";
                }

                return "Wellcome to secretNet!";
            }
        }        
    }
}