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
            return HexFromByteArray(mac);
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

            byte[] message = null;
            try
            {
                message = Convert.FromBase64String(messageBase64);
            }
            catch
            {
                return "Message is not valid base64 string!";
            }

            var computedMac = SHA1.Create().ComputeHash(key.Concat(message).ToArray());

            if (!this.CompareArrays(computedMac, this.StringToByteArray(mac)))
            {
                return "MAC is not valid!";
            }

            return Encoding.ASCII.GetString(message).Contains(";admin=true")
                       ? "Wellcome to SecretNet!"
                       : "You are not admin! Good bye!";
        }

        private byte[] GetSecretKey(string userId, string challengeId)
        {
            var hash = SHA256.Create();
            var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));

            var prg = new DeterministicCryptoRandomGenerator(seed, false);

            var keyLengthBytes = new byte[4];
            prg.GetBytes(keyLengthBytes);
            var keyLength = BitConverter.ToUInt32(keyLengthBytes, 0) % 64 + 1;

            var key = new byte[keyLength];
            prg.GetBytes(key);
            return key;
        }

        public static string HexFromByteArray(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        private static bool IsHexValid(string source)
        {
            var availableChars = "01234567890abcdef";
            if (source.ToLower().ToCharArray().Where(c => !availableChars.Contains(c)).Count() > 0
                || source.Length % 2 > 0)
            {
                return false;
            }

            return true;
        }


        private bool CompareArrays(byte[] first, byte[] second)
        {
            byte result = 0;
            if (first.Length != second.Length)
            {
                return false;
            }

            for (int i = 0; i < first.Length; i++)
            {
                result |= (byte)(first[i] ^ second[i]);
            }

            return result == 0;
        }

        public byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }
}