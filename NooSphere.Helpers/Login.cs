using System.Globalization;
using System.Web;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;

namespace NooSphere.Helpers
{
    public static class LoginHelper
    {
        public static IdentityProvider GetIdentityProvider(IdentityProviderName name)
        {
            return GetIdentityProviders().SingleOrDefault(ip => ip.Name == name.ToString());
        }

        private static List<IdentityProvider> GetIdentityProviders()
        {
            string identityProviderDiscovery =
                string.Format(CultureInfo.InvariantCulture,
                    "https://{0}.{1}/v2/metadata/IdentityProviders.js?protocol=javascriptnotify&realm={2}&version=1.0",
                    ConfigurationManager.AppSettings["ServiceNamespace"],
                    ConfigurationManager.AppSettings["ACSHostName"],
                    HttpUtility.UrlEncode(ConfigurationManager.AppSettings["TrustedAudience"]));

            string response = RestHelper.Get(identityProviderDiscovery);
            return JsonConvert.DeserializeObject<List<IdentityProvider>>(response);
        }
    }
    
    public class IdentityProvider
    {
        public string Name { get; set; }
        public string LoginUrl { get; set; }
    }

    public enum IdentityProviderName
    {
        Google
    }
}
