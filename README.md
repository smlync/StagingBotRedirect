# Azure Websites Staging Slots and Search Engine Bots
Setting up CI to Azure Websites is extremely easy and convenient. Whether you choose to do it from Git or Visual Studio Online, having a staging deployment slot makes this absolutely dead simple to ensure that you're putting high quality code into production so that a bad check in doesn't affect live users. The problem is that search engine bots are super greedy and manage to start crawling your staging site, which you definitely don't want - then real users may end up there. There are a few easy options you have to address this, and they aren't very well documented at the moment.

The first trick is getting the name of the site you're on, which you can do with the following:
    Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");

If you're on the staging site, this will return a value ending in -staging.azurewebsites.net. So now you need a place to check for this.

I chose to do this with an HTTP Module, partly because I'm old school and partly because I want this check to happen before it gets into any ASP.Net work that ultimately won't be necessary. So in the HTTP module, you add a BeginRequest event with code like so:

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
                    context.Response.RedirectPermanent("http://stagingbotredirector.azurewebsites.net/", true); 
                }
            }

To see this in action, use something that allows you to change your user agent to Googlebot and visit: http://stagingbotredirector-staging.azurewebsites.net

if you do use the HTTP module method, be sure to register the module in the web.config in the system.webServer modules section. The module itself will need to have a reference added to System.Web, and then add a reference to the module project from the web project.

#### Helpful links
1. [Azure Websites Deployment Slots - Explained](http://blog.amitapple.com/post/2014/11/azure-websites-slots/#.VNWBLfnF_zE)
2. [Detecting honest web crawlers](http://stackoverflow.com/questions/544450/detecting-honest-web-crawlers)
