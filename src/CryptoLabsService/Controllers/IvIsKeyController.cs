namespace CryptoLabsService.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    using CryptoLabsService.Helpers;
    using CryptoLabsService.Managers;

    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/IvIsKey")]
    public class IvIsKeyController : Controller
    {
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
            using (var hash = SHA256.Create())
            {
                var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));
                var token = TokenHelper.GetSecretTokenUser(seed);
                return token;
            }
        }

        [HttpGet]
        [Route("{userId}/{challengeId}/Admin/Token/")]
        public string GetAdminTokenHex([FromRoute] string userId, [FromRoute] string challengeId)
        {
            using (var hash = SHA256.Create())
            {
                var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));

                var token = TokenHelper.GetSecretTokenAdmin(seed);

                return token;
            }
        }

        [HttpGet]
        [Route("{userId}/{challengeId}/User/encryptedToken/hex")]
        public string GetEncryptedTokenHex([FromRoute] string userId, [FromRoute] string challengeId)
        {
            using (var hash = SHA256.Create())
            {
                var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));

                var token = TokenHelper.GetSecretTokenUser(seed);

                var data = Encoding.ASCII.GetBytes(token);
                var result = this.cbcIvIsKeyManager.EncryptCbc(
                    data.Concat(new byte[TokenHelper.BlockCount * TokenHelper.AesBlockSize - data.Length]).ToArray(),
                    seed,
                    false);
                return BitConverter.ToString(result).Replace("-", "");
            }
        }


        [HttpGet]
        [Route("{userId}/{challengeId}/authenticate/user/encryptedtoken={encryptedToken}")]
        public string AuthenticateUserEncrypted(
            [FromRoute] string userId,
            [FromRoute] string challengeId,
            [FromRoute] string encryptedToken)
        {
            using (var hash = SHA256.Create())
            {
                var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));

                byte[] encryptedTokenBytes;
                try
                {
                    encryptedTokenBytes = HexHelper.StringToByteArray(encryptedToken);
                }
                catch (Exception)
                {
                    return $"Invalid token format! Can not deformat token!";
                }

                byte[] decryptedTokenBytes;
                try
                {
                    decryptedTokenBytes = this.cbcIvIsKeyManager.DecryptCbc(encryptedTokenBytes, seed);
                }
                catch (Exception)
                {
                    return $"Invalid token format! Can not decrypt token!";
                }

                var token = Encoding.ASCII.GetString(decryptedTokenBytes);

                if (!TokenHelper.ValidateTokenString(token))
                {
                    return $"Invalid token format! Received token [{token}] contains invalid characters!. Raw token is [{BitConverter.ToString(decryptedTokenBytes).Replace("-", "")}]";
                }

                if (!TokenHelper.ValidateTokenUser(token, seed))
                {
                    return "Access denied";
                }

                return $"Wellcome {userId}!";
            }
        }

        [HttpGet]
        [Route("{userId}/{challengeId}/authenticate/admin/encryptedtoken={encryptedToken}")]
        public string AuthenticateAdminEncrypted(
            [FromRoute] string userId,
            [FromRoute] string challengeId,
            [FromRoute] string encryptedToken)
        {
            using (var hash = SHA256.Create())
            {
                var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));

                var encryptedTokenBytes = HexHelper.StringToByteArray(encryptedToken);
                var decryptedTokenBytes = this.cbcIvIsKeyManager.DecryptCbc(encryptedTokenBytes, seed);
                var token = Encoding.ASCII.GetString(decryptedTokenBytes);

                if (!TokenHelper.ValidateTokenString(token))
                {
                    return $"Invalid token format! Received token [{token}] contains invalid characters! Raw token is [{BitConverter.ToString(decryptedTokenBytes).Replace("-", "")}]";
                }

                if (!TokenHelper.ValidateTokenAdmin(token, seed))
                {
                    return "Access denied";
                }

                return $"Wellcome to secretNet!";
            }
        }
    }
}