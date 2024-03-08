using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using University.Models;

namespace University
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        
        public CustomAuthorizeAttribute()
        {
            
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            if (httpContext.Request.Cookies["ASP.NET_SessionId"] != null)
            {
                httpContext.Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                httpContext.Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            if (httpContext.Request.Cookies["AuthToken"] != null)
            {
                httpContext.Response.Cookies["AuthToken"].Value = string.Empty;
                httpContext.Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }



            var userPrinciple = httpContext.User as ClaimsPrincipal;
            var domainID = userPrinciple.Claims.FirstOrDefault(c => c.Type == "preferred_username").Value;


            if (domainID == null || domainID == "")
            {
                authorize = false;
            }
            else
            {






                pesEntities db = new pesEntities();
                httpContext.Session["UserName"] = domainID;
                httpContext.Session["UserID"] = (from u in db.Users where u.UserName == domainID select u.UserId).FirstOrDefault();
               // httpContext.Session["Password"] = (from u in db.Users where u.UserName == domainID select u.Password).FirstOrDefault();
                httpContext.Session["UserGroup"] = (from u in db.Users where u.UserName == domainID select u.UserGroup).FirstOrDefault();


                if (httpContext.Session["UserID"] == null || httpContext.Session["UserID"].ToString() == "0" || httpContext.Session["UserGroup"] == null || httpContext.Session["UserGroup"].ToString() == "0" || httpContext.Session["UserName"] == null || httpContext.Session["UserName"].ToString() == "0")
                {
                    authorize = false;
                }

                //Begin session regenerate
                var Context = System.Web.HttpContext.Current;
                System.Web.SessionState.SessionIDManager manager = new System.Web.SessionState.SessionIDManager();
                string oldId = manager.GetSessionID(Context);
                string newId = manager.CreateSessionID(Context);
                bool isAdd = false, isRedir = false;
                manager.SaveSessionID(Context, newId, out isRedir, out isAdd);
                HttpApplication ctx = (HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
                HttpModuleCollection mods = ctx.Modules;
                System.Web.SessionState.SessionStateModule ssm = (SessionStateModule)mods.Get("Session");
                System.Reflection.FieldInfo[] fields = ssm.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                SessionStateStoreProviderBase store = null;
                System.Reflection.FieldInfo rqIdField = null, rqLockIdField = null, rqStateNotFoundField = null;
                foreach (System.Reflection.FieldInfo field in fields)
                {
                    if (field.Name.Equals("_store")) store = (SessionStateStoreProviderBase)field.GetValue(ssm);
                    if (field.Name.Equals("_rqId")) rqIdField = field;
                    if (field.Name.Equals("_rqLockId")) rqLockIdField = field;
                    if (field.Name.Equals("_rqSessionStateNotFound")) rqStateNotFoundField = field;
                }
                object lockId = rqLockIdField.GetValue(ssm);
                if ((lockId != null) && (oldId != null)) store.ReleaseItemExclusive(Context, oldId, lockId);
                rqStateNotFoundField.SetValue(ssm, true);
                rqIdField.SetValue(ssm, newId);
                //End session regenerate



                // now create a new cookie with this guid value 
                /*
                HttpCookie currentUserCookie2 = Request.Cookies["AuthToken"];
                Response.Cookies.Remove("AuthToken");
                currentUserCookie2.Expires = DateTime.Now.AddDays(-10);
                currentUserCookie2.Value = null;
                Response.SetCookie(currentUserCookie2);
                Response.Cookies.Remove("AuthToken");
                       
               */
                authorize = true;
            }
            return authorize;
        }
        protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
        {
            
            filterContext.Result = new RedirectResult("~/Home/error");
        }
    }
}