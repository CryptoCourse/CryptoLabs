using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace CryptoLabsService.Labs.CbcIvIsTime
{
    [ApiController]
    [Route("api/IvIsTime")]
    public class IvIsTimeController : Controller
    {
        private readonly CbcIvIsTimeManager cbcIvIsTimeManager;

        public IvIsTimeController(CbcIvIsTimeManager cbcIvIsTimeManager)
        {
            this.cbcIvIsTimeManager = cbcIvIsTimeManager;
        }

        // GET api/
        [HttpGet]
        public string Get()
        {
            return "operating";
        }

        [HttpGet]
        [Route("{userId}/{challengeId}/encryptedpin")]
        public string GetEncryptedPin([FromRoute] string userId, [FromRoute] string challengeId)
        {
            using (var hash = SHA256.Create())
            {
                var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));

                var pin = this.GetPin(seed);

                var data = Encoding.ASCII.GetBytes(pin.ToString());
                var result = this.cbcIvIsTimeManager.EncryptCbc(
                    data.Concat(new byte[16 - data.Length]).ToArray(),
                    seed,
                    false);
                return Convert.ToBase64String(result);
            }
        }

        [HttpPost]
        [Route("{userId}/{challengeId}/noentropy")]
        public string PostNoEnctropy([FromRoute] string userId, [FromRoute] string challengeId, [FromBody] string value)
        {
            using (var hash = SHA256.Create())
            {
                var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));
                var data = Convert.FromBase64String(value);
                var result = this.cbcIvIsTimeManager.EncryptCbc(data, seed, false);
                return Convert.ToBase64String(result);
            }
        }

        [HttpGet]
        [Route("time")]
        public string GetTime([FromRoute] string userId, [FromRoute] string challengeId)
        {
            var time = this.cbcIvIsTimeManager.GetIv();
            return Convert.ToBase64String(time);
        }

        [HttpGet]
        [Route("{userId}/{challengeId}/validate")]
        public string GetVerify([FromRoute] string userId, [FromRoute] string challengeId)
        {
            using (var hash = SHA256.Create())
            {
                var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));

                var pin = this.GetPin(seed);

                var data = Encoding.ASCII.GetBytes(pin.ToString());

                return Convert.ToBase64String(data.Concat(new byte[16 - data.Length]).ToArray());
            }
        }

        private int GetPin(byte[] seed)
        {
            // why not
            return 39 * seed[7] + seed[12];
        }
    }
}