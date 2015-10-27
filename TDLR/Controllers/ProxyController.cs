using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tdlr.Utils;

namespace Tdlr.Controllers
{
    public class ProxyController : Controller
    {
        private static string discoveryTemplate = ConfigHelper.AadInstance + "/.well-known/openid-configuration";

        // GET: Proxy
        public async Task<ActionResult> Discovery(string domain)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, String.Format(CultureInfo.InvariantCulture, discoveryTemplate, domain));
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Search(string query)
        {
            if (HttpContext.Request.IsAjaxRequest())
            {
                string token = GraphHelper.AcquireToken(ClaimsPrincipal.Current.FindFirst(Globals.ObjectIdClaimType).Value);
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, query);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.SendAsync(request);

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

            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }
    }
}