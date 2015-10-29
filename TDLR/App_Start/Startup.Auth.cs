using System;
using System.Web;

//The following libraries were added to this sample.
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security.ActiveDirectory;
using Owin;

//The following libraries were defined and added to this sample.
using Tdlr.Utils;
using System.Globalization;
using Microsoft.Owin.Security.Notifications;
using Microsoft.IdentityModel.Protocols;
using Tdlr.DAL;

namespace Tdlr
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            //Configure OpenIDConnect, register callbacks for OpenIDConnect Notifications
            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = ConfigHelper.ClientId,
                    Authority = String.Format(CultureInfo.InvariantCulture, ConfigHelper.AadInstance, "common"),
                    PostLogoutRedirectUri = ConfigHelper.PostLogoutRedirectUri,
                    RedirectUri = ConfigHelper.PostLogoutRedirectUri,
                    TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = false
                    },
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        AuthorizationCodeReceived = OnAuthorizationCodeReceived,
                        AuthenticationFailed = OnAuthenticationFailed,
                        RedirectToIdentityProvider = OnRedirectToIdentityProvider,
                    }
                });

            app.UseWindowsAzureActiveDirectoryBearerAuthentication(new WindowsAzureActiveDirectoryBearerAuthenticationOptions
            {
                Audience = "https://strockisdevtwo.onmicrosoft.com/tdlr",
                Tenant = "strockisdevtwo.onmicrosoft.com",
                TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = false,
                }
            });
        }

        private Task OnRedirectToIdentityProvider(RedirectToIdentityProviderNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification)
        {
            if (notification.Request.Path.Value.ToLower() == "/account/signup/aad")
            {
                notification.ProtocolMessage.Prompt = "consent";
                string login_hint = notification.OwinContext.Authentication.AuthenticationResponseChallenge.Properties.Dictionary["login_hint"];
                notification.ProtocolMessage.LoginHint = login_hint;
            }

            return Task.FromResult(0);
        }

        private Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedNotification notification)
        {
            ClientCredential credential = new ClientCredential(ConfigHelper.ClientId, ConfigHelper.AppKey);
            string userObjectId = notification.AuthenticationTicket.Identity.FindFirst(Globals.ObjectIdClaimType).Value;
            string tenantId = notification.AuthenticationTicket.Identity.FindFirst(Globals.TenantIdClaimType).Value;
            AuthenticationContext authContext = new AuthenticationContext(String.Format(CultureInfo.InvariantCulture, ConfigHelper.AadInstance, tenantId), new TokenDbCache(userObjectId));
            AuthenticationResult result = authContext.AcquireTokenByAuthorizationCode(
                notification.Code, new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)), credential, ConfigHelper.GraphResourceId);
            return Task.FromResult(0);
        }

        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification)
        {
            notification.HandleResponse();
            notification.Response.Redirect("/Error/ShowError?signIn=true&errorMessage=" + notification.Exception.Message);
            return Task.FromResult(0);
        }
    }
}