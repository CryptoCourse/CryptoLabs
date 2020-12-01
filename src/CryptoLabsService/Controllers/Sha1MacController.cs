namespace CryptoLabsService.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;

    using CryptoLabsService.Helpers;

    using Microsoft.AspNetCore.Mvc;

    [Route("api/Sha1Mac")]
    public class Sha1MacController : Controller
    {
        private const string UserIdPattern = @"^\w+$";

        [HttpGet]
        public string Get()
        {
            return "operating";
        }

        [HttpGet]
        [Route("{userId}/{challengeId}/mac")]
        public string ComputeMac([FromRoute] string userId, [FromRoute] string challengeId)
        {
            if (!Regex.IsMatch(userId, UserIdPattern))
            {
                return "Invalid userId. UserId should contains only alphabetical and numerical symbols!";
            }

            var key = this.GetSecretKey(userId, challengeId);
            var message = Encoding.ASCII.GetBytes($"user={userId};");

            var mac = SHA1.Create().ComputeHash(key.Concat(message).ToArray());
            return HexHelper.HexFromByteArray(mac);
        }

        [HttpPost]
        [Route("{userId}/{challengeId}/{mac}/verify")]
        public string VerifyMac(
            [FromRoute] string userId,
            [FromRoute] string challengeId,
            [FromRoute] string mac,
            [FromBody] string messageBase64)
        {
            if (!Regex.IsMatch(userId, UserIdPattern))
            {
                return "Invalid userId. UserId should contains only alphabetical and numerical symbols!";
            }

            var key = this.GetSecretKey(userId, challengeId);

            byte[] message;
            try
            {
                message = Convert.FromBase64String(messageBase64);
            }
            catch
            {
                return "Message is not valid base64 string!";
            }

            var computedMac = SHA1.Create().ComputeHash(key.Concat(message).ToArray());

            if (!CompareHelper.CompareArrays(computedMac, HexHelper.StringToByteArray(mac)))
            {
                return "MAC is not valid!";
            }

            return Encoding.ASCII.GetString(message).Contains(";admin=true")
                       ? "Wellcome to SecretNet!"
                       : "You are not admin! Goodbye!";
        }

        private byte[] GetSecretKey(string userId, string challengeId)
        {
            using (var hash = SHA256.Create())
            {
                var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));

                using (var generator = new DeterministicCryptoRandomGenerator(seed, false))
                {
                    var keyLengthBytes = new byte[4];
                    generator.GetBytes(keyLengthBytes);
                    var keyLength = BitConverter.ToUInt32(keyLengthBytes, 0) % 64 + 1;

                    var key = new byte[keyLength];
                    generator.GetBytes(key);
                    return key;
                }
            }
        }
    }
}