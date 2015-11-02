using System;
using System.Web;
using System.Web.Mvc;

//The following libraries were added to this sample.
using System.Security.Claims;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.ActiveDirectory.GraphClient.Extensions;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

//The following libraries were defined and added to this sample.
using Tdlr.Utils;


namespace Tdlr.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult SignUp()
        {
            // Completes the sign up flow by pre-filing the sign up form using claims
            ViewBag.Email = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Name).Value;
            ViewBag.FirstName = ClaimsPrincipal.Current.FindFirst(Globals.GivennameClaimType).Value;
            ViewBag.LastName  = ClaimsPrincipal.Current.FindFirst(Globals.SurnameClaimType).Value;
            return View("Index");
        }
    }
}