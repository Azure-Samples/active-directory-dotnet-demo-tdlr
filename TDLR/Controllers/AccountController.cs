using System.Web;
using System.Web.Mvc;

//The following libraries were added to this sample.
using System.Security.Claims;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;

//The following libraries were defined and added to this sample.
using Tdlr.Utils;
using System;
using System.Globalization;
using Tdlr.DAL;

namespace Tdlr.Controllers
{
    public class AccountController : Controller
    {
        /// <summary>
        /// Sends an OpenIDConnect Sign-In Request.
        /// </summary>
        public ActionResult SignIn()
        {
            return View();
        }


        public void AADSignIn(string redirectUri)
        {
            if (redirectUri == null)
                redirectUri = "/";

            HttpContext.GetOwinContext()
                .Authentication.Challenge(new AuthenticationProperties { RedirectUri = redirectUri },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }

        /// <summary>
        /// Signs the user out and clears the cache of access tokens.
        /// </summary>
        public void SignOut()
        {
            // Remove all cache entries for this user and send an OpenID Connect sign-out request.
            if (Request.IsAuthenticated)
            {
                string userObjectID = ClaimsPrincipal.Current.FindFirst(Globals.ObjectIdClaimType).Value;
                string tenantId = ClaimsPrincipal.Current.FindFirst(Globals.TenantIdClaimType).Value;
                var authContext = new AuthenticationContext(String.Format(CultureInfo.InvariantCulture, ConfigHelper.AadInstance, tenantId), new TokenDbCache(userObjectID));
                authContext.TokenCache.Clear();

                HttpContext.GetOwinContext().Authentication.SignOut(
                    OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
            }
        }
    }
}