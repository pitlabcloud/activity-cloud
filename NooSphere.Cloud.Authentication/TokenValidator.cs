using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using NooSphere.Helpers;
    
namespace NooSphere.Cloud.Authentication
{
    public class TokenValidator
    {
        private string issuerLabel = "Issuer";
        private string expiresLabel = "ExpiresOn";
        private string audienceLabel = "Audience";
        private string hmacSHA256Label = "HMACSHA256";

        private string acsHostName;
        private string trustedSigningKey;
        private string trustedTokenIssuer;
        private string trustedAudienceValue;

        public TokenValidator(string acsHostName, string serviceNamespace, string trustedAudienceValue, string trustedSigningKey)
        {
            this.trustedSigningKey = trustedSigningKey;
            this.trustedTokenIssuer = String.Format("https://{0}.{1}/",
                serviceNamespace.ToLowerInvariant(),
                acsHostName.ToLowerInvariant());

            this.trustedAudienceValue = trustedAudienceValue;
        }

        public bool Validate(string token)
        {
            if (!this.IsHMACValid(token, Convert.FromBase64String(this.trustedSigningKey)))
                return false;

            if (this.IsExpired(token))
                return false;

            if (!this.IsIssuerTrusted(token))
                return false;

            if (!this.IsAudienceTrusted(token))
                return false;

            return true;
        }

        private static ulong GenerateTimeStamp()
        {
            // Default implementation of epoch time
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToUInt64(ts.TotalSeconds);
        }

        private bool IsAudienceTrusted(string token)
        {
            Dictionary<string, string> tokenValues = TokenHelper.GetNameValues(token);

            string audienceValue;

            tokenValues.TryGetValue(this.audienceLabel, out audienceValue);

            if (!string.IsNullOrEmpty(audienceValue))
                if (audienceValue.Equals(this.trustedAudienceValue, StringComparison.Ordinal))
                    return true;

            return false;
        }

        private bool IsIssuerTrusted(string token)
        {
            Dictionary<string, string> tokenValues = TokenHelper.GetNameValues(token);

            string issuerName;

            tokenValues.TryGetValue(this.issuerLabel, out issuerName);

            if (!string.IsNullOrEmpty(issuerName))
                if (issuerName.Equals(this.trustedTokenIssuer))
                    return true;

            return false;
        }

        private bool IsHMACValid(string swt, byte[] sha256HMACKey)
        {
            string[] swtWithSignature = swt.Split(new string[] { "&" + this.hmacSHA256Label + "=" }, StringSplitOptions.None);

            if ((swtWithSignature == null) || (swtWithSignature.Length != 2))
                return false;

            HMACSHA256 hmac = new HMACSHA256(sha256HMACKey);

            byte[] locallyGeneratedSignatureInBytes = hmac.ComputeHash(Encoding.ASCII.GetBytes(swtWithSignature[0]));

            string locallyGeneratedSignature = HttpUtility.UrlEncode(Convert.ToBase64String(locallyGeneratedSignatureInBytes));

            return locallyGeneratedSignature == swtWithSignature[1];
        }

        private bool IsExpired(string swt)
        {
            try
            {
                Dictionary<string, string> nameValues = TokenHelper.GetNameValues(swt);
                string expiresOnValue = nameValues[this.expiresLabel];
                ulong expiresOn = Convert.ToUInt64(expiresOnValue);
                ulong currentTime = Convert.ToUInt64(GenerateTimeStamp());

                if (currentTime > expiresOn)
                    return true;

                return false;
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException();
            }
        }
    }
}