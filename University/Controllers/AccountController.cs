using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using University.Controllers.Authentication;
using University.Cryptography;
using University.Models;

namespace University.Controllers
{
   [UserRightAttribute]
    public class AccountController : Controller
    {
       /*
        //
        // GET: /Account/
       private pesEntities db = new pesEntities(); 
        public ActionResult ChangePassword()
        {
            int userid=int.Parse(Session["UserID"].ToString());
            User User = (from u in db.Users
                                where u.UserId == userid
                                
                                select u).First();
            User.Password = "";

            return View(User);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(FormCollection collection)
        {
            int userid = int.Parse(Session["UserID"].ToString());
           
           
            User User = (from u in db.Users
                                where u.UserId == userid
                                select u).First();
           string password = AESCryptography.Decrypt(User.Password);
            if (Request["NewPassword"].ToString() != "")
            {
                if (Request["NewPassword"].ToString() != Request["ConfirmPassword"].ToString()) 
                {
                    TempData["ErrorMessage"] = "Passwords do not match.";
                    return View(User);
                }
                else if (Request["OldPassword"].ToString() != password)
                {
                    TempData["ErrorMessage"] = "Your old password is incorrect";
                    return View(User);
                }
               
                else
                {

                    User.Password = AESCryptography.Encrypt(Request["NewPassword"].ToString());
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Password has been successfully changed. Please login with your new password.";

                    return RedirectToAction("Login", "Home", null);
                }
            }
            else 
            {
                TempData["ErrorMessage"] = "Please fill new password properly.";
                return View(User);
            }
           
        }

        public ActionResult Account()
        {
            return View(db.Users.ToList().OrderBy(e=>e.UserName));
        }
        public ActionResult UserEdit(int? UserId)
        {
            if (UserId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User up = db.Users.Find(UserId);
            if (up == null)
            {
                return HttpNotFound();
            }
          
            ViewBag.stat1 = up.AccountStatus;
            ViewBag.UserGroup = new SelectList(db.luUserGroups, "Id", "Group",up.UserGroup);
            return View(up);
        }
        public ActionResult UserEdit2(int? UserId)
        {
            if (UserId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User up = db.Users.Find(UserId);
            if (up == null)
            {
                return HttpNotFound();
            }
            TempData["Wizard3"] = "Yes";
           
            ViewBag.stat1 = up.AccountStatus;
            ViewBag.UserGroup = new SelectList(db.luUserGroups, "UserGroupId", "UserGroupName", up.UserGroup);
            return View("UserEdit", up);
        }
        public ActionResult UserEditWizard(int? UserId)
        {
            if (UserId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IEnumerable<User> up = db.Users.ToList();
            if (up == null)
            {
                return HttpNotFound();
            }
             TempData["Wizard2"] = "Yes";
             TempData["continue"] = "Edit Account";
             TempData["UserId"] = UserId;
            
            return View("Account", up);
        }
        public ActionResult UserEditWizard2(int? UserId)
        {
            if (UserId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User up = db.Users.Find(UserId);
            if (up == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserGroup = new SelectList(db.luUserGroups, "UserGroupId", "UserGroupName", up.UserGroup);
            TempData["Wizard2"] = "Yes";
            return RedirectToAction("UserEdit", up);
        }

        // POST: /Benefit/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
      



       
        // GET: /Account/Create
        public ActionResult UserCreate()
        {
            //ViewBag.Branch = new SelectList(db.luBranches, "BranchId", "BranchName");
            ViewBag.UserGroup = new SelectList(db.luUserGroups, "UserGroupId", "UserGroupName");
            return View("UserCreate");
        }

        // POST: /UsrProfile/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserCreate([Bind(Include = "UserId,UserName,UserGroup")] User userprofile)
        {
            userprofile.Password = "123";
            userprofile.AccountStatus = 0;
            
            if (!userprofile.UserName.Contains("."))
            {
                TempData["ErrorMessage"] = "User name must be the same as your email address, example yoftahe.suleiman";
                return RedirectToAction("UserCreate", "Account");
           
            }
            else if (ModelState.IsValid)
            {
                userprofile.Password = AESCryptography.Encrypt(userprofile.Password);
                try { 
                db.Users.Add(userprofile);

                ActivityLog al = new ActivityLog();
                al.logdate = DateTime.Now;
                al.UserName = Session["UserName"].ToString();
                al.TableName = "UserProfile";
                al.Activity = "Created user account for " + userprofile.UserName;
                al.IP = Request.UserHostAddress;
                db.ActivityLogs.Add(al);

                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    var sqlException = e.InnerException.InnerException as System.Data.SqlClient.SqlException;

                    if (sqlException.Number == 2601 || sqlException.Number == 2627)
                    {
                        TempData["ErrorMEssage"] = "Cannot insert duplicate values.";
                    }
                    else
                    {
                        TempData["ErrorMEssage"] = "Error while saving data.";
                    }
                    TempData["continue"] = "Create Account";
                    return RedirectToAction("Account");
                }
                //TempData["continue"] = "Create Account";




                TempData["UserId"] = userprofile.UserId;

               // TempData["SuccessMessage"] = "Account for " + userprofile.UserName + " has been created. Now create employee detail to " + userprofile.UserName;
                return RedirectToAction("EmployeeDetail", "EmployeeDetail", new {userid = userprofile.UserId,username=userprofile.UserName,wizard = "yes"});
            
            }

         //   ViewBag.Branch = new SelectList(db.luBranches, "BranchId", "BranchName", userprofile.Branch);
            ViewBag.UserGroup = new SelectList(db.luUserGroups, "UserGroupId", "UserGroupName", userprofile.UserGroup);
            return RedirectToAction("Login", "Home");
        }
        [NonAction]
        private void sendEmail(string to, string subject, string message)
        {

            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress(to));
            msg.From = new MailAddress("Import.portal@zemenbank.com");
            msg.Subject = subject;

            //  DataSet dsNAme = da.selectFullNAme(empId);
            // lblFullNAme.Text = dsNAme.Tables[0].Rows[0][0].ToString();
            msg.Body = message;
            msg.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("Import.portal@zemenbank.com", "P@ssw0rd");
            client.Port = 25; // You can use Port 25 if 587 is blocked (mine is!)
            client.Host = "mail.zemenbank.com";
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = false;

            client.Send(msg);
           
        }
       
     
        [HttpGet]
        public ActionResult resetPassword(string id)
        {
            if (id == null)
            {
                return View();
            }
            else
            {
                string usr = AESCryptography.Decrypt(id);
                User User = (from u in db.Users
                                    where u.UserName == usr
                                    select u).First();
                if (User != null)
                {
                    User.Password = "CKhcn7gjLQ5rIMhzes/r2A==";
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Password has been successfuly reset.Your new password is 123.";
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = "User name does not exist.Please try again.";
                    return View();
                }
            }
           
        }
      

       */
       
    }
}
