using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Azure.ActiveDirectory.GraphClient.Extensions;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Tdlr.DAL;

namespace Tdlr.Utils
{
    class GraphHelper
    {
        public static string AcquireToken(string userObjectId)
        {
            ClientCredential cred = new ClientCredential(ConfigHelper.ClientId, ConfigHelper.AppKey);
            string tenantId = ClaimsPrincipal.Current.FindFirst(Globals.TenantIdClaimType).Value;
            AuthenticationContext authContext = new AuthenticationContext(String.Format(CultureInfo.InvariantCulture, ConfigHelper.AadInstance, tenantId), new TokenDbCache(userObjectId));
            AuthenticationResult result = authContext.AcquireTokenSilent(ConfigHelper.GraphResourceId, cred, new UserIdentifier(userObjectId, UserIdentifierType.UniqueId));
            return result.AccessToken;
        }
    }
}
