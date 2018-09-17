namespace CryptoLabsService.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    using CryptoLabsService.Helpers;
    using CryptoLabsService.Managers;

    using Microsoft.AspNetCore.Mvc;

    [Route("api/IvIsKey")]
    public class IvIsKeyController : Controller
    {
        private const int AesBlockSize = 16;

        const string AvailableTokenChars = "0123456789abcdefghijklmnopqrstuvwxyz-";

        private readonly CbcIvIsKeyManager cbcIvIsKeyManager;

        public IvIsKeyController(CbcIvIsKeyManager cbcIvIsKeyManager)
        {
            this.cbcIvIsKeyManager = cbcIvIsKeyManager;
        }

        // GET api/
        [HttpGet]
        public string Get()
        {
            return "operating";
        }

        [HttpGet]
        [Route("{userId}/{challengeId}/User/Token/")]
        public string GetTokenHex([FromRoute] string userId, [FromRoute] string challengeId)
        {
            var hash = SHA256.Create();

            var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));

            var token = this.GetSecretTokenUser(seed);

            return token;
        }

        [HttpGet]
        [Route("{userId}/{challengeId}/Admin/Token/")]
        public string GetAdminTokenHex([FromRoute] string userId, [FromRoute] string challengeId)
        {
            var hash = SHA256.Create();

            var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));

            var token = this.GetSecretTokenAdmin(seed);

            return token;
        }

        [HttpGet]
        [Route("{userId}/{challengeId}/User/encryptedToken/hex")]
        public string GetEncryptedTokenHex([FromRoute] string userId, [FromRoute] string challengeId)
        {
            var hash = SHA256.Create();

            var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));

            var token = this.GetSecretTokenUser(seed);

            var data = Encoding.ASCII.GetBytes(token);
            var result = this.cbcIvIsKeyManager.EncryptCbc(
                data.Concat(new byte[16 - data.Length]).ToArray(),
                seed,
                false);
            return BitConverter.ToString(result).Replace("-", "");
        }


        [HttpGet]
        [Route("{userId}/{challengeId}/authenticate/user/encryptedtoken={encryptedToken}")]
        public string AuthenticateUserEncrypted(
            [FromRoute] string userId,
            [FromRoute] string challengeId,
            [FromRoute] string encryptedToken)
        {
            var hash = SHA256.Create();
            var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));

            var encryptedTokenBytes = this.StringToByteArray(encryptedToken);
            var decryptedTokenBytes = this.cbcIvIsKeyManager.DecryptCbc(encryptedTokenBytes, seed);
            var token = Encoding.ASCII.GetString(decryptedTokenBytes);

            if (!this.ValidateTokenString(token))
            {
                return $"Invalid token format! Received token [{token}] contains invalid characters!";
            }

            if (!this.ValidateTokenUser(token, seed))
            {
                return "Access denied";
            }

            return $"Wellcome {userId}!";
        }

        [HttpGet]
        [Route("{userId}/{challengeId}/authenticate/admin/encryptedtoken={encryptedToken}")]
        public string AuthenticateAdminEncrypted(
            [FromRoute] string userId,
            [FromRoute] string challengeId,
            [FromRoute] string encryptedToken)
        {
            var hash = SHA256.Create();
            var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));

            var encryptedTokenBytes = this.StringToByteArray(encryptedToken);
            var decryptedTokenBytes = this.cbcIvIsKeyManager.DecryptCbc(encryptedTokenBytes, seed);
            var token = Encoding.ASCII.GetString(decryptedTokenBytes);

            if (!this.ValidateTokenString(token))
            {
                return $"Invalid token format! Received token [{token}] contains invalid characters!";
            }

            if (!this.ValidateTokenAdmin(token, seed))
            {
                return "Access denied";
            }

            return $"Wellcome to secterNet!";
        }

        private string GetSecretTokenUser(byte[] seed)
        {
            
            using (var generator = new DeterministicCryptoRandomGenerator(seed, false))
            {
                // need 3 blocks for attack, extra - for fun
                var bytes = new byte[AesBlockSize * 4];
                generator.GetBytes(bytes);
                var chars = bytes
                    .Select(b => AvailableTokenChars[b % AvailableTokenChars.Length]);
                var token = new string(chars.ToArray());
                return token;
            }
        }

        private string GetSecretTokenAdmin(byte[] seed)
        {

            using (var generator = new DeterministicCryptoRandomGenerator(seed, false))
            {
                // need 3 blocks for attack, extra - for fun
                var bytes = new byte[AesBlockSize * 4];
                generator.GetBytes(bytes);
                var chars = bytes
                    .Select(b => AvailableTokenChars[b % AvailableTokenChars.Length]);
                var token = new string(chars.ToArray());
                return token;
            }
        }

        private bool ValidateTokenString(string token)
        {
            return token.All(l => AvailableTokenChars.Contains(l));
        }

        private bool ValidateTokenAdmin(string token, byte[] seed)
        {
            return this.GetSecretTokenAdmin(seed).SequenceEqual(token);
        }

        private bool ValidateTokenUser(string token, byte[] seed)
        {
            return this.GetSecretTokenUser(seed).SequenceEqual(token);
        }

        private byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }
}