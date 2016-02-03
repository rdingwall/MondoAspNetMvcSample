using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mondo;
using MondoAspNetMvcSample.Models;

namespace MondoAspNetMvcSample.Controllers
{
    public sealed class HomeController : Controller
    {
        private readonly IMondoAuthorizationClient _mondoAuthorizationClient;

        public HomeController()
        {
            string clientId = ConfigurationManager.AppSettings["MondoClientId"];
            string clientSecret = ConfigurationManager.AppSettings["MondoClientSecret"];

            _mondoAuthorizationClient = new MondoAuthorizationClient(clientId, clientSecret, "https://production-api.gmon.io");
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            string state = CryptoHelper.GenerateRandomString(64);

            Session["state"] = state;

            // send user to Mondo's login page
            return Redirect(_mondoAuthorizationClient.GetAuthorizeUrl(state, Url.Action("OAuthCallback", "Home", null, Request.Url.Scheme)));
        }

        [HttpGet]
        public async Task<ActionResult> OAuthCallback(string code, string state)
        {
            // verify anti-CSRF state token matches what was sent
            string expectedState = Session["state"] as string;

            if (!string.Equals(expectedState, state))
            {
                throw new SecurityException("State mismatch");
            }

            Session.Remove("state");

            // exchange authorization code for access token
            AccessToken accessToken = await _mondoAuthorizationClient.GetAccessTokenAsync(code, Url.Action("OAuthCallback", "Home", null, Request.Url.Scheme));
            
            // fetch transactions etc
            using (var client = new MondoClient(accessToken.Value, "https://production-api.gmon.io"))
            {
                IList<Account> accounts = await client.GetAccountsAsync();
                Balance balance = await client.GetBalanceAsync(accounts[0].Id);
                IList<Transaction> transactions = await client.GetTransactionsAsync(accounts[0].Id, expand: "merchant");

                return View(new AccountSummaryModel { Account = accounts[0], Balance = balance, Transactions = transactions });
            }
        }
    }
}