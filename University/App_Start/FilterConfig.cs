using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace University
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class NoDirectAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
           
            if (HttpContext.Current.Session["AuthToken"] != null && HttpContext.Current.Request.Cookies["AuthToken"] != null && HttpContext.Current.Request.Cookies["AuthToken"].Value != string.Empty && HttpContext.Current.Request.Cookies["AuthToken"].Value != "" && !HttpContext.Current.Session["AuthToken"].ToString().Equals(HttpContext.Current.Request.Cookies["AuthToken"].Value))
            {
                filterContext.Result = new RedirectToRouteResult(new
                                           RouteValueDictionary(new { controller = "Home", action = "Logout", area = "Main" }));

            }

            if (filterContext.HttpContext.Request.UrlReferrer == null ||
     filterContext.HttpContext.Request.Url.Host != filterContext.HttpContext.Request.UrlReferrer.Host)
            {
                filterContext.Result = new RedirectToRouteResult(new
                                          RouteValueDictionary(new { controller = "Home", action = "Logout", area = "Main" }));
            }

            
        }
    }
}
