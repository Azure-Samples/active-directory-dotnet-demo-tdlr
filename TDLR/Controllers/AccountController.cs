using System.Web;
using System.Web.Mvc;

//The following libraries were added to this sample.
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;

//The following libraries were defined and added to this sample.
using System.Collections.Generic;

namespace Tdlr.Controllers
{
    public class AccountController : Controller
    {
        // Show the app's sign in page
        public ActionResult SignIn()
        {
            return View();
        }

        // Handle when the user clicks the sign up button
        public ActionResult SignUp()
        {
            if (!Request.IsAuthenticated)
            {
                return Redirect("/Error/ShowError?errorMessage=" + "This is just a sample app, we didn't actually implement local account sign up... :-)");
            }

            return Redirect("/tasks");
        }

        // Send a sign in request to AAD
        public void AADSignIn(string redirectUri)
        {
            if (redirectUri == null)
                redirectUri = "/";

            HttpContext.GetOwinContext()
                .Authentication.Challenge(new AuthenticationProperties { RedirectUri = redirectUri },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }

        // Send a sign in request to AAD
        public void AADSignUp(string redirectUri, string sign_up_hint)
        {
            if (redirectUri == null)
                redirectUri = "/";

            // Pass some additional data along with the request, in this case, to pre-populate the username field
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict["login_hint"] = sign_up_hint;

            HttpContext.GetOwinContext()
                .Authentication.Challenge(new AuthenticationProperties (dict) { RedirectUri = redirectUri },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }

        public void SignOut()
        {
            if (Request.IsAuthenticated)
            {
                // Send a sign out request to AAD
                HttpContext.GetOwinContext().Authentication.SignOut(
                    OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
            }
        }
    }
}