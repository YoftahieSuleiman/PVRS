/*****************************************************/
/*Copyright(c) Zemen Bank S.C.                      */
/* All Rights Reserved                               */
/* An Unpublished Work                               */
/*                                                   */
/* This is a Proprietary program product material    */
/*and is the property of Zemen Bank S.C.            */
/* No sale, reproduction or other use of this        */
/* program product is authorized except as granted   */
/* by the fully executed Zemen Bank SC product      */
/* license or by the separate written agreement      */
/* and approval of Zemen Bank S.C.                  */
/*****************************************************/
/* Author: Yoftahie Suleiman                                 */
/* Revision History:                                 */
/* Version 1.0         */
/* Date Created 8/2/17

/*****************************************************/
/* Purpose:                                          */
/* --------                                          */
/* This file contains the controller for the 
 * Login and index pages.This is the starting point of
 * the export application */
/*****************************************************/

using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.SessionState;
using University.Controllers.Authentication;
using University.Cryptography;
using University.Models;
//using University.ViewModels;

namespace University.Controllers
{
    [HandleError]
    [Authorize]
    public class HomeController : Controller
    {
        public static string strConn = ConfigurationManager.ConnectionStrings["FlexConnection"].ConnectionString;
        pesEntities db = new pesEntities();


        [HttpGet]
        [Authorize]
        public JsonResult RefreshSession()
        {
            // Perform the refresh token exchange here and return updated tokens
            // Remember to validate the request and add proper security checks.

            HttpContext.GetOwinContext().Authentication.Challenge();

            return Json(new { msg = "chalenge page requested." }, JsonRequestBehavior.AllowGet); // Return the refreshed tokens
        }

        public ActionResult Personal(int id)
        {
            using (pesEntities db = new pesEntities())
            {
                string user = Session["UserName"].ToString();
                User up = (from u in db.Users
                              where u.UserId == id
                          select u ).First();
            return View("Personal", up);
             
            }
            
        }
        public ActionResult Education(int id)
        {
            using (pesEntities db = new pesEntities())
            {
                string user = Session["UserName"].ToString();
                User up = (from u in db.Users
                                  where u.UserId == id
                                  select u).First();
                return View("Education", up);

            }

        }

     
        
     
        public ActionResult Home()
        {

          
            // if user is not logged in then redirect to log in page
            if (Session["UserName"] == null)
            {
               
                return RedirectToAction("Login");
            }

            using (pesEntities db = new pesEntities())
            {
                string user = Session["UserName"].ToString();
                if(Session["UserGroup"].ToString()=="3") //If branch_user
                {
                    string branchname = Session["BranchName"].ToString();

                    List<EstimationRequest> reqs = (from d in db.EstimationRequests
                                                   where d.RequestingBranch == branchname
                                                   orderby d.DateUploaded descending
                                                   select d).Include(d=>d.luDocumentStatu).Include(u=>u.User).Include(u=>u.User1).Include(u=>u.User2).Include(p=>p.luPropertyType).Include(d=>d.luCustomerType).Include(p=>p.luValuationPurpose).ToList();
                    return View(reqs);
                }
              else if(Session["UserGroup"].ToString()=="2") //If engineering officer
                {
                    int userid = int.Parse(Session["UserID"].ToString());
                   
                    List<EstimationRequest> reqs = (from d in db.EstimationRequests
                                                    where d.AssignedTo == userid
                                                    orderby d.DateUploaded descending
                                                    select d).Include(d => d.luDocumentStatu).Include(u => u.User).Include(u => u.User1).Include(u => u.User2).Include(p => p.luPropertyType).Include(d => d.luCustomerType).Include(p => p.luValuationPurpose).ToList();
              
                    return View(reqs);
                }
              else if(Session["UserGroup"].ToString()=="1") //If engineering admin
                {
                    int userid = int.Parse(Session["UserID"].ToString());
                   
                    List<EstimationRequest> reqs = (from d in db.EstimationRequests
                                                    where (d.Status > 1 )
                                                    orderby d.Status
                                                    orderby d.DateUploaded descending
                                                    select d).Include(d => d.luDocumentStatu).Include(u => u.User).Include(u => u.User1).Include(u => u.User2).Include(p => p.luPropertyType).Include(d => d.luCustomerType).Include(p => p.luValuationPurpose).ToList();
             
                    return View(reqs);
                }
                else if (Session["UserGroup"].ToString() == "5") //If engineering csr
                {
                    int userid = int.Parse(Session["UserID"].ToString());

                    List<EstimationRequest> reqs = (from d in db.EstimationRequests
                                                    where (d.Status > 1)
                                                    orderby d.Status
                                                    orderby d.DateUploaded descending
                                                    select d).Include(d => d.luDocumentStatu).Include(u => u.User).Include(u => u.User1).Include(u => u.User2).Include(p => p.luPropertyType).Include(d => d.luCustomerType).Include(p => p.luValuationPurpose).ToList();

                    return View(reqs);
                }
                else //If credit user
                {
                    List<int> requeststat = new List<int>() { 4,6};
                    List<EstimationRequest> reqs = (from d in db.EstimationRequests
                                                    where requeststat.Contains(d.Status)
                                                    orderby d.DateUploaded descending
                                                    select d).Include(d => d.luDocumentStatu).Include(u => u.User).Include(u => u.User1).Include(u => u.User2).Include(p => p.luPropertyType).Include(d => d.luCustomerType).Include(p => p.luValuationPurpose).ToList();
             
                    return View(reqs);
                }
          } 
               
             
            

            
            return View("Home");
        }


        // GET: /City/Edit/5
  

        public ActionResult OidcCallback()
        {
            // Determine the dynamic callback path based on some logic.
            bool authorize = false;
            //if (Request.Cookies["ASP.NET_SessionId"] != null)
            //{
            //    Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
            //    Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            //}

            //if (Request.Cookies["AuthToken"] != null)
            //{
            //    Response.Cookies["AuthToken"].Value = string.Empty;
            //    Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            //}



            var userPrinciple = User as ClaimsPrincipal;
            var domainID = userPrinciple.Claims.FirstOrDefault(c => c.Type == "preferred_username").Value;


            if (domainID == null || domainID == "")
            {
                authorize = false;
            }
            else
            {






               


                if (Session["UserID"] == null || Session["UserID"].ToString() == "0" || Session["UserGroup"] == null || Session["UserGroup"].ToString() == "0" || Session["UserName"] == null || Session["UserName"].ToString() == "0")
                {
                    authorize = false;
                }
                University.Models.pesEntities db = new pesEntities();
                Session["UserName"] = domainID;
                User user = (from u in db.Users where u.UserName == domainID select u).FirstOrDefault();
                Session["UserID"] = user.UserId;
                Session["UserGroup"] = user.UserGroup;
                Session["Division"] = user.Division;

                authorize = true;
            }

            if (authorize)
            {

                //string redirectAction = HttpUtility.UrlDecode(Request.Cookies["redirectAction"].Value.ToString());

                /*
                if (redirectAction != null && redirectAction.Length >= 0 && !redirectAction.Equals("/") && !redirectAction.Equals("/HRIS_test/", StringComparison.OrdinalIgnoreCase) && !redirectAction.Equals("/HRIS/", StringComparison.OrdinalIgnoreCase) && !redirectAction.Equals("/HRIS_test", StringComparison.OrdinalIgnoreCase) && !redirectAction.Equals("/HRIS", StringComparison.OrdinalIgnoreCase) && !redirectAction.Equals("/Home/oidccallback", StringComparison.OrdinalIgnoreCase) && !redirectAction.Equals("/HRIS_test/Home/oidccallback", StringComparison.OrdinalIgnoreCase))
                {
                    return Redirect(redirectAction);
                }
                else
                {
                    return RedirectToAction("Home");
                }
                */
                HRISDbHandle dbConnection = new HRISDbHandle();
                dbConnection.Open();
                DataTable userlocation = dbConnection.GetTable("SELECT Top(1) a.UserId,b.[Name] as [Department],c.[Name] as [BankingCenter] FROM [HRIS].[dbo].[EmployeeExperience] a  join [HRIS].[dbo].luDepartment b on  a.Department=b.Id  left join [HRIS].[dbo].luBankingCenter c on a.BankingCenter=c.Id where UserId=(select top(1) UserId from [HRIS].[dbo].UserProfile where [UserName] like '%" + Session["UserName"].ToString() + "%' ) and ExperienceType=1 and [To] is null");
                foreach (DataRow row in userlocation.Rows)
                {
                    Session["Department"] = row["Department"].ToString().Trim();
                    Session["BranchName"] = row["BankingCenter"].ToString().Trim();
                }
                dbConnection.Close();

                
                string bc="";
                if (Session["BranchName"] != null && Session["BranchName"].ToString() != "")
                {
                 bc=Session["BranchName"].ToString();
                string qry = "select Branch_code from fcubsprd.sttm_branch where lower(replace(branch_name,' ','')) like lower(replace('%"+bc+"%',' ',''))";
            using (OleDbConnection connection = new OleDbConnection(strConn))
            {
                OleDbCommand command = new OleDbCommand(qry);

                // Set the Connection to the new OleDbConnection.
                command.Connection = connection;
                // Open the connection and execute the insert command.
                try
                {
                    connection.Open();
                    OleDbDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        if (!reader.IsClosed)
                        {
                            while (reader.Read())
                            {
                                Session["BranchCode"] = reader["Branch_code"].ToString();

                            }
                        }
                        
                    }
                   
                    connection.Close();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            }
               
                return RedirectToAction("Home");
            }
            else
            {
                return RedirectToAction("error");
            }

            // Redirect to the appropriate action method using the dynamic callback path.

        }
        [AllowAnonymous]

        public ActionResult Error()
        {
            Session["UserName"] = null;
            Session["UserID"] = null;
            Session["UserGroup"] = null;
            Session["BranchName"] = null;
            Session["BranchCode"] = null;
            Session["Department"] = null;
            Session["Division"] = null;
            return View();
        }
        //--------New Code Added For Login  --------------//
        public ActionResult Login()
        {

            return RedirectToAction("OidcCallback");

        }

        public ActionResult Logout()
        {

            //Logout code
            FormsAuthentication.SignOut();
            Session.Clear();
            Response.Cookies.Remove("ASP.NET_SessionId");
            Response.Cookies.Remove("ASP.NET_SessionId");
            Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            //AUTH_SESSION_ID
            Session.RemoveAll();

            //Session.Abandon();


            if (Request.Cookies["ASP.NET_SessionId"] != null && Request.Cookies["ASP.NET_SessionId"].Value != "")
            {
                Response.Cookies["ASP.NET_SessionId"].Value = "";
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);

            }

            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }

            Session.Abandon();
            System.Web.Security.FormsAuthentication.SignOut();
            HttpContext.GetOwinContext().Authentication.SignOut(
                new AuthenticationProperties
                {
                    RedirectUri = ConfigurationManager.AppSettings["oidc:RedirectUrl"]
                },
                    CookieAuthenticationDefaults.AuthenticationType,
                    OpenIdConnectAuthenticationDefaults.AuthenticationType
                );


            //Logout code
            return RedirectToAction("Login", "Home");
        }
        public ActionResult Unauthorized()
        {
            return View();
        }
       
        public ActionResult NotFound()
        {
            return View();
        }

      
             
     
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        
        //public ActionResult Login(User objUser)
        //{

            
            
           
        //    if (ModelState.IsValid)
        //    {


        //        using (pesEntities db = new pesEntities())
        //        {
                  
        //            if (objUser.Password == null || objUser.UserName == null)
        //            {
        //                TempData["msg"]  = "Fill user name & password properly";

        //                return RedirectToAction("Login");
        //            }
        //            string PassWord = AESCryptography.Encrypt(objUser.Password);
        //            User user = (from u in db.Users
        //                                where u.UserName == objUser.UserName
        //                                where u.Password == PassWord
        //                                select u).FirstOrDefault();
        //            if (user != null)
        //            {
        //                //Cache User Information
        //                Session["UserID"] = user.UserId.ToString();
        //                Session["UserName"] = user.UserName.ToString();
        //                Session["Password"] = user.Password.ToString();
        //               // Session["UserName"] = user.FullName.ToString();
        //                //Session["Branch"] = user.Branch.ToString();
        //                Session["UserGroup"] = user.Group.ToString();
                     
                       
        //                    return RedirectToAction("Home");
                       
        //            }
        //            else
        //            {
        //                TempData["msg"] = "Enter A valid user name & password";                                               
        //                return RedirectToAction("Login");
        //            }
        //        }
        //    }
        //    return View(objUser);
        //}



     
        //------------End Log in code -------------------//

        

        
      
        //UpdatePrerequisiteStatus
        //UpdatePrerequisiteStatus
       


        //Fetch Training Names
        public JsonResult reloadSession(int statusid) // its a GET, not a POST
        {
            string session = "";
            // In reality you will do a database query based on the value of provinceId, but based on the code you have shown
            try
            {
                if (Session["ClosedNotifications"] == null)
                {
                    Session["ClosedNotifications"] = statusid;
                }
                else
                {
                    Session["ClosedNotifications"] = Session["ClosedNotifications"] + "," + statusid;
                }
               // Debug.WriteLine("############################################ Index:" + statusid + " , Session" + Session["ClosedNotifications"]);
                session = Session["ClosedNotifications"].ToString();
            }
            catch (Exception)
            {
            }


            return Json(session, JsonRequestBehavior.AllowGet);


         
        }
      



    }
}