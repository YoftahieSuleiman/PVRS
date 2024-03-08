using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Management;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using University.Models.ModelBinders;

namespace University
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new DecimalModelBinder());
        }
        private void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            var httpException = ex as HttpException ?? ex.InnerException as HttpException;
            if (httpException == null) return;

            if (httpException.WebEventCode == WebEventCodes.RuntimeErrorPostTooLarge)
            {
                //handle the error
                Response.Write("Too big a file, dude"); //for example
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            /*
            if (Request.HttpMethod != "POST" && Session["UserID"] == null)
            {

                Response.Cookies.Remove("ASP.NET_SessionId");
                Response.Cookies.Remove("AuthToken");

                Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
                Response.Cookies.Add(new HttpCookie("AuthToken", ""));
                Session["UserID"] = null;

                Session["UserName"] = null;

                Session["Password"] = null;
                //Session["UserName"] = user.FullName.ToString();
                //Session["Branch"] = user.Branch.ToString();
                Session["UserGroup"] = null;
                string guid = Guid.NewGuid().ToString();
                Session["AuthToken"] = guid;
                Response.Cookies.Add(new HttpCookie("AuthToken", guid));
                Response.Cookies.Remove("ASP.NET_SessionId");
                string sessionid = Session.SessionID;

                string a = "2ab6cde4fg5hij7klmn8op9qrs3tuvwxyz01";
                Random rnd = new Random();
                int rndindex = rnd.Next(1, 5);
                int txtstart = rnd.Next(0, 25);
                string randomstring = a.Substring(txtstart, 10);

                sessionid = sessionid.Substring(0, rndindex) + randomstring + sessionid.Substring(rndindex);
                Response.Cookies.Remove("ASP.NET_SessionId");
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-11);
                Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", sessionid));
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(20);
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(20);
            }
            */
        }
        protected void Session_End(object sender, EventArgs e)
        {

        }
       
    }
}
