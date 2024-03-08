using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using University.Models;



using System.Configuration;
using System.Net;
using System.Web.Security;



namespace University.Controllers.Authentication
{
  
    [AttributeUsage(AttributeTargets.All)]
    public class UserRightAttribute : ActionFilterAttribute
    {
        pesEntities db = new pesEntities();
        [NonAction]
        public List<string> GetActivities(int Group_ID)
        {
            int usergroup = int.Parse(HttpContext.Current.Session["UserGroup"].ToString());
            var query1 = from p in db.luGrants
                         where p.UserGroupId == Group_ID
                         select p.ActivityId;

            var query2 = from d in db.luUserActivities
                         where query1.Contains(d.ActivityId)
                         select d.ACtivityName;

            var results2 = query2.ToList();
            return results2;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

           
            Log(filterContext);
           // filterContext.Controller.ViewData.Model = bindingContext.Model;
          //  base.OnActionExecuting(filterContext, bindingContext);
        }
        private void Log(ActionExecutingContext filterContext)
        {

            string actionName = filterContext.RouteData.Values["action"].ToString();
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string usergroup = "";
            if (HttpContext.Current.Session["UserGroup"] != null)
            {
                usergroup = HttpContext.Current.Session["UserGroup"].ToString();
            }
            else
            {
                new RouteValueDictionary(new { controller = "Home", action = "Login" });
            }


            if (usergroup != "")
            {
                List<string> allowed = GetActivities(int.Parse(usergroup));
               

                //if (allowed.Contains(actionName))
                //{


                 
                //    filterContext.Result = new ViewResult
                //    {
                //        ViewName = actionName,
                //        ViewData = filterContext.Controller.ViewData
                //    };
                  
                //}
                if (!allowed.Contains(actionName))
                {
                    filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Home", action = "Unauthorized" })
                    );
                    // filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
                    filterContext.Result = new ViewResult
                    {
                        ViewName = "Unauthorized",
                        ViewData = filterContext.Controller.ViewData
                    };

                }
               
                

            }
            else
            {
                new RouteValueDictionary(new { controller = "Home", action = "Login" });
            }

            
            
        }


     
    }

}