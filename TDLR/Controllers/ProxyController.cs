using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using Tdlr.Utils;

namespace Tdlr.Controllers
{
    public class ProxyController : Controller
    {
        private static string discoveryTemplate = ConfigHelper.AadInstance + "/.well-known/openid-configuration";
        private const string issuerTemplate = "https://sts.windows.net/{0}/";
        private List<string> ignoredTenants = new List<string> {
            String.Format(issuerTemplate, "9cd80435-793b-4f48-844b-6b3f37d1c1f3"),
            String.Format(issuerTemplate, "f8cdef31-a31e-4b4a-93e4-5f571e91255a")
        };

        // Sends a request to the AAD discovery endpoint to determine if a tenant exists with the given domain
        public async Task<ActionResult> Discovery(string domain)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, String.Format(CultureInfo.InvariantCulture, discoveryTemplate, domain));
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                string issuer = (string)JObject.Parse(responseString)["issuer"];

                // Ignore the two tenants that contain consumer domains, like gmail.com
                if (!ignoredTenants.Contains(issuer))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Search(string query)
        {
            // Instead of making Graph API CORS requests from javascript directly,
            // here we proxy the calls from the server.  This saves us from having to share
            // access tokens with the client code.

            if (HttpContext.Request.IsAjaxRequest())
            {
                try
                {
                    // Get a token from the ADAL cache
                    string token = GraphHelper.AcquireToken(ClaimsPrincipal.Current.FindFirst(Globals.ObjectIdClaimType).Value);

                    // Send the Graph API query
                    HttpClient client = new HttpClient();
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, query);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    HttpResponseMessage response = await client.SendAsync(request);

                    // Return the JSON from the Graph API directly to the client
                    if (response.IsSuccessStatusCode)
                    {
                        string responseString = await response.Content.ReadAsStringAsync();
                        return Content(responseString, "application/json");
                    }
                    else
                    {
                        return new HttpStatusCodeResult(response.StatusCode);
                    }
                }
                catch (AdalException ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
        }
    }
}