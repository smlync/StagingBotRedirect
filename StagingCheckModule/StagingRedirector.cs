using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace StagingCheckModule
{
    public class StagingRedirector : IHttpModule
    {
        public void Dispose()
        {
            // Can do something here if needed
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(this.BeginRequest);
        }

        public void BeginRequest(object sender, EventArgs e)
        {
            // Get the current request
            HttpContext context = ((HttpApplication)sender).Context;
            if (context == null) return;

            HttpRequest request = context.Request;

            string CurrentEnv = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");
            if (CurrentEnv != null && CurrentEnv.ToLower().EndsWith("-staging.azurewebsites.net"))
            {
                // Detect if the user is a bot
                if (IsBot(request.ServerVariables["HTTP_USER_AGENT"]))
                {
                    // Redirect the bot to the live website, **change to your site name**.
                    context.Response.RedirectPermanent("http://www.yoursite.com", true); 
                }
            }
        }

        private static bool IsBot(string UserAgent)
        {
            return Regex.IsMatch(UserAgent, @"bot|crawler|baiduspider|80legs|ia_archiver|voyager|curl|wget|yahoo! slurp|mediapartners-google", RegexOptions.IgnoreCase);
        }
    }
}
