using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using University.Models;

namespace University.Controllers
{
    public class UploadController : Controller
    {
        pesEntities db = new pesEntities();
        public JsonResult FetchPropertyTypes(string customerType) // its a GET, not a POST
        {
            // In reality you will do a database query based on the value of provinceId, but based on the code you have shown
            if (customerType == "1" || customerType == "3")
            {
                string[] types = { "Residential building", "Condominium", "Apartment", "Vehicle" };
                var names = from p in db.luPropertyTypes
                            where types.Contains(p.Type)
                            orderby p.Type
                            select new
                            {
                                p.Id,
                                p.Type
                            }; ;



                return Json(names, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var names = from p in db.luPropertyTypes
                            orderby p.Type
                            select new
                            {
                                p.Id,
                                p.Type
                            }; ;



                return Json(names, JsonRequestBehavior.AllowGet);
            }
           
        }
        // GET: Upload
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            else
            {
                List<UploadedDocument> docs = (from d in db.UploadedDocuments
                                               where d.EstimationRequestId == id.Value
                                               select d).ToList();
                EstimationRequest er = (from e in db.EstimationRequests
                                        where e.Id == id
                                        select e).FirstOrDefault();
                TempData["requestid"] = id;
                TempData["branch"] = er.RequestingBranch;
                TempData["applicant"] = er.ApplicantName;
                TempData["date"] = er.DateUploaded.ToString("MMM dd,yyyy");
                return View(docs);
            }
            
        }
         public ActionResult Upload(int? id)
        {

            if (id == null)
            {
                return HttpNotFound();
            }
            else
            {
                TempData["requestId"] = id;
                EstimationRequest er= (from e in db.EstimationRequests where e.Id==id select e).FirstOrDefault();
                TempData["branch"] = er.RequestingBranch;
                TempData["applicant"] = er.ApplicantName;
                TempData["requestdate"] = er.DateUploaded.ToString("MMM dd, yyyy");
                List<string> uploadedDocuments = (from u in db.UploadedDocuments
                                                  where u.EstimationRequestId == id
                                                  select u.luDocumentType.Name).ToList();
                if (uploadedDocuments.Contains("Other"))
                {
                    string otherRemark = (from u in db.UploadedDocuments
                                               where u.EstimationRequestId == id
                                               where u.DocumentTypeId == 6
                                               select u.Remark).FirstOrDefault();
                    int itemindex=uploadedDocuments.IndexOf("Other");
                    uploadedDocuments[itemindex] += "_" + otherRemark;

                }
                ViewBag.UploadedDocuments = uploadedDocuments;
                int[] documenttypes=new int[]{1,2,3,4,5,6,7,8,9,10};
                if(er.CustomerType==1 && (er.PropertyType==1 ||  er.PropertyType==2 ||  er.PropertyType==3)) //If customer is SLA && property type is residntial,condominium or appartment
  {
  documenttypes=new int[]{1,2,3,4,5,6};
  }
  else if(er.CustomerType==1 && er.PropertyType==7) //If customer is SLA && property type is vehicle
  {
  documenttypes=new int[]{1,2,3,7,5,6};
  
  }
  else if(er.CustomerType==2 && (er.PropertyType==1 || er.PropertyType==2 || er.PropertyType==3 || er.PropertyType==4 || er.PropertyType==5 || er.PropertyType==6)) //If customer is Business && property type is not vehicle or machinery
  {
    documenttypes=new int[]{2,3,4,6};

 
  }
   else if(er.CustomerType==2 && er.PropertyType==7) //If customer is Business && property type is vehicle
  {
     documenttypes=new int[]{ 2,3,7,6};

  }
    else if(er.CustomerType==2 && er.PropertyType==8) //If customer is Business && property type is machinery
  {
       documenttypes=new int[]{ 2,3,8,9,6};
  }

    else if(er.CustomerType==3 && (er.PropertyType==1 || er.PropertyType==2 || er.PropertyType==3)) //If customer is zemen staff && property type is residential,cond or apartment
  {
         documenttypes=new int[]{ 10,2,4,5,6};
  }

    else if(er.CustomerType==3 && er.PropertyType==7) //If customer is zemen staff && property type is vehicle
  {
      documenttypes = new int[] { 10, 2, 7, 5, 6 };
 
  }

                List<int> uploaded = (from u in db.UploadedDocuments
                                      where u.EstimationRequestId == id
                                      select u.DocumentTypeId).ToList();

                ViewBag.DocumentTypeId = new SelectList(db.luDocumentTypes.Where(d => documenttypes.Contains(d.Id)).Where(u => !uploaded.Contains(u.Id)).OrderBy(b => b.Name), "Id", "Name");
                return View();
            }

        }
        [HttpPost]
         public JsonResult Upload()
         {

             int reqid = int.Parse(Request["EstimationRequestId"]);
             int requestid = int.Parse(Request["EstimationRequestId"]);
             EstimationRequest er = (from e in db.EstimationRequests where e.Id == requestid select e).FirstOrDefault();
             if (Request.Files["File"].ContentLength > 0)
             {
                 //string name = Convert.ToString(Session["LogedUserID"]);
                 string extension = System.IO.Path.GetExtension(Request.Files["File"].FileName);
                 string FileName = requestid + "_" + Request["DocumentTypeId"]+extension;

                 string path1 = string.Format("{0}/{1}", Server.MapPath(@"~\Content\UploadedDocuments\"), (FileName));
                 if (System.IO.File.Exists(FileName))
                 {
                     System.IO.File.Delete(FileName);

                 }
                 try
                 {
                     UploadedDocument ud = new UploadedDocument();
                     ud.EstimationRequestId = requestid;
                     ud.FilePath = "../Content/UploadedDocuments/" + FileName;
                     ud.DocumentTypeId = int.Parse(Request["DocumentTypeId"]);
                     ud.Remark = ud.DocumentTypeId == 6 ? Request["Remark"] : "";
                     db.UploadedDocuments.Add(ud);
                     db.SaveChanges();
                     Request.Files["File"].SaveAs(path1);

                     if (checkDocumentStatus(requestid))
                     {
                        
                         er.Status = 2;
                         er.DateUploaded = DateTime.Now;
                         db.Entry(er).State = EntityState.Modified;
                         db.SaveChanges();
                         List<int> groups = new List<int>(){1,5};
                         List<string> receivers = (from e in db.Users where groups.Contains(e.UserGroup) select e.UserName).ToList();
                         foreach (string receiver in receivers)
                         {
                             string subject = "New valuation request received";
                             string to = receiver + "@zemenbank.com";
                             string message = "<b>Dear " + receiver + "</b>,<br /> Valuation is requested by " + er.RequestingBranch + " for " + er.ApplicantName + ".<br /><ul><li>Property type : " + er.luPropertyType.Type + "</li><li>Customer type : " + er.luCustomerType.Type + "</li><li>Address: " + er.EstimationAddress1 + ", " + er.EstimationAddress2 + "</li></ul>";
                             sendEmail(to, subject, message);
                         }
                     }
                     else
                     {
                         
                         er.Status = 1;
                         db.Entry(er).State = EntityState.Modified;
                         db.SaveChanges();
                     }

                 }
                 catch
                 {
                     return Json("Error Occured while processing your request. Please try again.");
                     
                 }
                 List<string> uploadedDocuments = (from u in db.UploadedDocuments
                                                   where u.EstimationRequestId == requestid
                                                   select u.luDocumentType.Name).ToList();
                 string doclist = "";
                 if (uploadedDocuments.Contains("Other"))
                 {
                     string otherRemark = (from u in db.UploadedDocuments
                                           where u.EstimationRequestId == requestid
                                           where u.DocumentTypeId == 6
                                           select u.Remark).FirstOrDefault();
                     int itemindex = uploadedDocuments.IndexOf("Other");
                     uploadedDocuments[itemindex] += "_" + otherRemark;

                 }
                 if (uploadedDocuments.Count() > 0)
                 {
                     doclist+="<ul>";
                     foreach (string doctype in uploadedDocuments)
                     {
                         doclist+="<li>" + doctype + "</li>";
                     }

                     doclist+="</ul>";

                 }
                 else
                 {
                     doclist+="<p style='color:red'>No documents uploaded for this request</p>";
                 }
             return Json(doclist, JsonRequestBehavior.AllowGet);
             }
             return Json("Error Occured while processing your request. Please try again.");
             
             
             

         }

        public ActionResult FinalReport(int? id)
        {

            if (id == null)
            {
                return HttpNotFound();
            }
            else
            {
                TempData["requestId"] = id;
                EstimationRequest er = (from e in db.EstimationRequests where e.Id == id select e).FirstOrDefault();
                TempData["branch"] = er.RequestingBranch;
                TempData["applicant"] = er.ApplicantName;
                TempData["requestdate"] = er.DateUploaded.ToString("MMM dd, yyyy");
                TempData["CSRRemark"] = er.CSRRemark;
               
                return View();
            }

        }
        [HttpPost]
        public JsonResult FinalReport()
        {

            int reqid = int.Parse(Request["EstimationRequestId"]);
            int requestid = int.Parse(Request["EstimationRequestId"]);
            EstimationRequest er = (from e in db.EstimationRequests where e.Id == requestid select e).FirstOrDefault();
            if (Request.Files["File"] != null && Request.Files["File"].ContentLength > 0)
            {
                //string name = Convert.ToString(Session["LogedUserID"]);
                string extension = System.IO.Path.GetExtension(Request.Files["File"].FileName);
                string FileName = requestid +"_Final_Report" + extension;

                string path1 = string.Format("{0}/{1}", Server.MapPath(@"~\Content\UploadedDocuments\"), (FileName));
                if (System.IO.File.Exists(FileName))
                {
                    System.IO.File.Delete(FileName);

                }
                Request.Files["File"].SaveAs(path1);
                er.EstimationReport = "../Content/UploadedDocuments/" + FileName;
            }
            else
            {
                er.EstimationReport = "";
            }

                try
                {


                        er.Status = 6;
                        er.CSRRemark = Request["CSRRemark"];
                        db.Entry(er).State = EntityState.Modified;
                        db.SaveChanges();
                        string subject = "Valuation request completed";
                        string messagebranch = "<b>Dear " + er.User2.UserName + "</b>,<br /> Valuation Request is completed.<br /><ul><li>Applicant : " + er.ApplicantName + "</li><li>Property type : " + er.luPropertyType.Type + "</li><li>Customer type : " + er.luCustomerType.Type + "</li><li>Address: " + er.EstimationAddress1 + ", " + er.EstimationAddress2 + "</li><li>Valuation Date: " + er.ValuationDate.Value.ToString("MMM dd,yyyy") + " , " + er.ValuationTime + "</li><li>Final Remark: " + er.CSRRemark + "</li></ul>";
                        sendEmail(er.User2.UserName + "@zemenbank.com", subject, messagebranch);

                }
                catch
                {
                    return Json("Error Occured while processing your request. Please try again.");

                }
                List<string> uploadedDocuments = (from u in db.UploadedDocuments
                                                  where u.EstimationRequestId == requestid
                                                  select u.luDocumentType.Name).ToList();
                
                return Json("Successfully added final report", JsonRequestBehavior.AllowGet);




        }
        [NonAction]
        private void sendEmail(string to, string subject, string message)
        {

            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress(to));
            msg.From = new MailAddress("Import.portal@zemenbank.com");
            msg.Subject = subject;

            msg.Body = message;
            msg.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("Import.portal@zemenbank.com", "P@ssw0rd");
            client.Port = 25; // You can use Port 25 if 587 is blocked (mine is!)
            client.Host = "smtp.zemenbank.com";
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = false;

            client.Send(msg);

        }


        public ActionResult ChangeDoc(int? reqid, int? docid)
        {

            if (reqid == null || docid==null)
            {
                return HttpNotFound();
            }
            else
            {
                TempData["path"] = reqid + "_" + docid + ".pdf";
                //TempData["path"] = string.Format("{0}/{1}", Server.MapPath(@"~\Content\UploadedDocuments\"), (reqid + "_" + docid + ".pdf"));

                return View();
            }

        }
        [HttpPost]
        public JsonResult ChangeDoc()
        {
            if (Request["path"] != "" && Request["path"] != null)
            {
                if (Request.Files["File"].ContentLength > 0)
                {
                    //string name = Convert.ToString(Session["LogedUserID"]);

                    string path1 = string.Format("{0}/{1}", Server.MapPath(@"~\Content\UploadedDocuments\"), (Request["path"]));

                    if (System.IO.File.Exists(path1))
                    {
                        System.IO.File.Delete(path1);

                    }
                    try
                    {
                       
                        Request.Files["File"].SaveAs(path1);


                    }
                    catch
                    {
                        return Json("Error Occured while processing your request. Please try again.");

                    }
                    
                    return Json("File successfully changed", JsonRequestBehavior.AllowGet);
                }
            }
            

            
            return Json("Error Occured while processing your request. Please try again.");




        }


        public ActionResult RemoveDoc(int? reqid, int? docid)
        {

            if (reqid == null || docid == null )
            {
                return HttpNotFound();
            }
            else
            {
                TempData["path"] = reqid + "_" + docid + ".pdf";
                TempData["documentid"] = docid;
                
                
                //TempData["path"] = string.Format("{0}/{1}", Server.MapPath(@"~\Content\UploadedDocuments\"), (reqid + "_" + docid + ".pdf"));

                return View();
            }

        }
        [HttpPost]
        public JsonResult RemoveDoc()
        {
            if (Request["path"] != "" && Request["path"] != null && Request["documentid"] != null && Request["documentid"] != "" )
            {
                
                    //string name = Convert.ToString(Session["LogedUserID"]);

                    string path1 = string.Format("{0}/{1}", Server.MapPath(@"~\Content\UploadedDocuments\"), (Request["path"]));

                    if (System.IO.File.Exists(path1))
                    {
                        System.IO.File.Delete(path1);

                    }
                    try
                    {
                        int docid = int.Parse(Request["documentid"]);
                        UploadedDocument d = (from u in db.UploadedDocuments
                                              where u.Id == docid
                                              select u).FirstOrDefault();
                        int reqid=d.EstimationRequestId;
                        db.UploadedDocuments.Remove(d);
                        db.SaveChanges();
                        if (checkDocumentStatus(reqid))
                        {
                            EstimationRequest er = (from e in db.EstimationRequests
                                                    where e.Id == reqid
                                                    select e).FirstOrDefault();
                            er.Status = 2;
                            db.Entry(er).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            EstimationRequest er = (from e in db.EstimationRequests
                                                    where e.Id == reqid
                                                    select e).FirstOrDefault();
                            er.Status = 1;
                            db.Entry(er).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                    }
                    catch
                    {
                        return Json("Error Occured while processing your request. Please try again.");

                    }

                    return Json("File successfully removed", JsonRequestBehavior.AllowGet);
                
            }



            return Json("Error Occured while processing your request. Please try again.");




        }

        public bool checkDocumentStatus(int requestId)
        {
            EstimationRequest er = (from e in db.EstimationRequests
                                    where e.Id == requestId
                                    select e).FirstOrDefault();
            List<int> uploadeddocs = (from u in db.UploadedDocuments
                                      where u.EstimationRequestId == requestId
                                      select u.DocumentTypeId).ToList();

            if (uploadeddocs.Count() == 0)
            {
                return false;
            }
            int[] sofaruploaded = uploadeddocs.ToArray();


            int[] documenttypes = new int[] {};
            int[] optionals = new int[] { 5,6};
            if (er.CustomerType == 1 && (er.PropertyType == 1 || er.PropertyType == 2 || er.PropertyType == 3)) //If customer is SLA && property type is residntial,condominium or appartment
            {
                documenttypes = new int[] { 1, 2, 3, 4 };
            }
            else if (er.CustomerType == 1 && er.PropertyType == 7) //If customer is SLA && property type is vehicle
            {
                documenttypes = new int[] { 1, 2, 3, 7 };

            }
            else if (er.CustomerType == 2 && (er.PropertyType == 1 || er.PropertyType == 2 || er.PropertyType == 3 || er.PropertyType == 4 || er.PropertyType == 5 || er.PropertyType == 6)) //If customer is Business && property type is not vehicle or machinery
            {
                documenttypes = new int[] { 2, 3, 4 };


            }
            else if (er.CustomerType == 2 && er.PropertyType == 7) //If customer is Business && property type is vehicle
            {
                documenttypes = new int[] { 2, 3, 7 };

            }
            else if (er.CustomerType == 2 && er.PropertyType == 8) //If customer is Business && property type is machinery
            {
                documenttypes = new int[] { 2, 3, 8, 9 };
            }

            else if (er.CustomerType == 3 && (er.PropertyType == 1 || er.PropertyType == 2 || er.PropertyType == 3)) //If customer is zemen staff && property type is residential,cond or apartment
            {
                documenttypes = new int[] { 10, 2, 4 };
            }

            else if (er.CustomerType == 3 && er.PropertyType == 7) //If customer is zemen staff && property type is vehicle
            {
                documenttypes = new int[] { 10, 2, 7 };

            }
            sofaruploaded = sofaruploaded.Where(s => !optionals.Contains(s)).ToArray();

            return (sofaruploaded.Length == documenttypes.Length && sofaruploaded.Intersect(documenttypes).Count() == sofaruploaded.Length);

        }
        public ActionResult CreateRequest()
        {
            ViewBag.CustomerType = new SelectList(db.luCustomerTypes.OrderBy(b=>b.Type), "Id", "Type");
            ViewBag.PropertyType = new SelectList(db.luPropertyTypes.OrderBy(b => b.Type), "Id", "Type");
            ViewBag.ValuationPurpose = new SelectList(db.luValuationPurposes.OrderBy(b => b.Purpose), "Id", "Purpose");
            return View();
        }

        [HttpPost]
        public ActionResult CreateRequest([Bind(Include = "Id,ApplicantName,MortgagerName,EstimationAddress1,EstimationAddress2,HardCopyFiles,ManagerResponse,CustomerType,PropertyType,ValuationPurpose,HardCopyStatus,CSRRemark,EstimationReport")] EstimationRequest estimationRequest, FormCollection form, HttpPostedFileBase doc1)
        {
            ViewBag.CustomerType = new SelectList(db.luCustomerTypes.OrderBy(b => b.Type), "Id", "Type", estimationRequest.CustomerType);
            ViewBag.PropertyType = new SelectList(db.luPropertyTypes.OrderBy(b => b.Type), "Id", "Type", estimationRequest.PropertyType);
            ViewBag.ValuationPurpose = new SelectList(db.luValuationPurposes.OrderBy(b => b.Purpose), "Id", "Purpose", estimationRequest.ValuationPurpose);

            if (Session["UserId"] == null || Session["BranchName"] == null || Session["UserGroup"].ToString() != "3" || estimationRequest.ApplicantName == "" || estimationRequest.MortgagerName == "" || estimationRequest.EstimationAddress1 == "" || estimationRequest.EstimationAddress2 == "")
            {
                return RedirectToAction("Home");
            }
            estimationRequest.DateUploaded = DateTime.Today;
            estimationRequest.UploadedBy = int.Parse(Session["UserId"].ToString());
            estimationRequest.Status = 1;
            estimationRequest.ManagerResponse = "";
            estimationRequest.HardCopyStatus = "";
            estimationRequest.CSRRemark = "";
            estimationRequest.EstimationReport = "";
            estimationRequest.RequestingBranch = Session["BranchName"].ToString();
            try
            {
                db.EstimationRequests.Add(estimationRequest);
                db.SaveChanges();
                return RedirectToAction("Home", "Home");
            }
            catch(Exception e)
            {
                string error = e.Message;
                return RedirectToAction("Home");
            }
            return View();
        }

        public ActionResult ValuationRequestDetail(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EstimationRequest estimationRequests = db.EstimationRequests.Find(id);
            if (estimationRequests == null)
            {
                return HttpNotFound();
            }

            List<UploadedDocument> docs = (from d in db.UploadedDocuments
                                           where d.EstimationRequestId == estimationRequests.Id
                                           select d).ToList();
            ViewBag.docs=docs;
            return View(estimationRequests);
        }

        public ActionResult Assign(int? reqid)
        {

            if (reqid == null)
            {
                return HttpNotFound();
            }
            else
            {
                EstimationRequest er = (from e in db.EstimationRequests
                                        where e.Id == reqid
                                        select e).FirstOrDefault();
                TempData["requestinfo"]="Valuation request by "+er.RequestingBranch+ " for "+er.ApplicantName+ " on "+er.DateUploaded.ToString("MMM dd, yyyy");
                TempData["requestid"] = er.Id;
                TempData["assignedto"] = er.AssignedTo;
                ViewBag.userid = new SelectList(db.Users.Where(u=>u.UserGroup==2).OrderBy(b => b.UserName), "UserId", "UserName",er.AssignedTo);

                return View();
            }

        }
        [HttpPost]
        public JsonResult Assign()
        {
            if (Request["requestid"] != "" && Request["requestid"] != null && Request["userid"] != "" && Request["userid"] != null)
            {
                
                    //string name = Convert.ToString(Session["LogedUserID"]);

                    
                    try
                    {

                        int requestid = int.Parse(Request["requestid"]);
                        int userid = int.Parse(Request["userid"]);
                        EstimationRequest er = (from e in db.EstimationRequests
                                                where e.Id == requestid
                                                select e).FirstOrDefault();
                        er.AssignedTo = userid;
                        er.AssignedBy = int.Parse(Session["UserID"].ToString());
                        er.Status = 3;
                        string valdate = Request["ValuationDate"];
                        er.ValuationDate = DateTime.Parse(Request["ValuationDate"]);
                        er.DateAssigned = DateTime.Today;
                        er.ValuationTime = Request["ValuationTime"];
                        db.Entry(er).State = EntityState.Modified;
                        db.SaveChanges();
                        er = (from e in db.EstimationRequests
                              where e.Id == requestid
                              select e).FirstOrDefault();
                        string subject = "Valuation request assigned";
                        string messageassigned = "<b>Dear " + er.User1.UserName + "</b>,<br /> Valuation Request is assigned to you.<br /><ul><li>Applicant : " + er.ApplicantName + "</li><li>Property type : " + er.luPropertyType.Type + "</li><li>Customer type : " + er.luCustomerType.Type + "</li><li>Address: " + er.EstimationAddress1 + ", " + er.EstimationAddress2 + "</li><li>Valuation Date: " + er.ValuationDate.Value.ToString("MMM dd,yyyy") + " , " + er.ValuationTime + "</li></ul>";
                        string messagebranch = "<b>Dear " + er.User2.UserName + "</b>,<br /> Valuation Request is assigned to "+er.User1.UserName+".<br /><ul><li>Applicant : " + er.ApplicantName + "</li><li>Property type : " + er.luPropertyType.Type + "</li><li>Customer type : " + er.luCustomerType.Type + "</li><li>Address: " + er.EstimationAddress1 + ", " + er.EstimationAddress2 + "</li><li>Valuation Date: " + er.ValuationDate.Value.ToString("MMM dd,yyyy") + " , " + er.ValuationTime + "</li></ul>";
                        sendEmail(er.User1.UserName + "@zemenbank.com", subject, messageassigned);
                        sendEmail(er.User2.UserName + "@zemenbank.com", subject, messagebranch);



                    }
                    catch
                    {
                        return Json("Error Occured while processing your request. Please try again.");

                    }

                    return Json("Successfully assigned to " + Request["username"], JsonRequestBehavior.AllowGet);
                
            }



            return Json("Error Occured while processing your request. Please try again.");




        }
        public ActionResult Respond(int? reqid)
        {

            if (reqid == null)
            {
                return HttpNotFound();
            }
            else
            {
                EstimationRequest er = (from e in db.EstimationRequests
                                        where e.Id == reqid
                                        select e).FirstOrDefault();
                
                TempData["response"] = er.ManagerResponse;
                TempData["requestid"] = er.Id;
                return View();
            }

        }
        [HttpPost]
        public JsonResult Respond()
        {
            if (Request["requestid"] != "" && Request["requestid"] != null && Request["managerResponse"] != "" && Request["managerResponse"] != null)
            {

                //string name = Convert.ToString(Session["LogedUserID"]);


                try
                {

                    int requestid = int.Parse(Request["requestid"]);
                    EstimationRequest er = (from e in db.EstimationRequests
                                            where e.Id == requestid
                                            select e).FirstOrDefault();
                    er.ManagerResponse = Request["managerResponse"];
                    er.AssignedBy = null;
                    er.AssignedTo = null;
                    er.Status = 2;
                    db.Entry(er).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch
                {
                    return Json("Error Occured while processing your request. Please try again.");

                }

                return Json("Successfully assigned to " + Request["username"], JsonRequestBehavior.AllowGet);

            }



            return Json("Error Occured while processing your request. Please try again.");




        }

        public ActionResult Evaluate(int? reqid)
        {

            if (reqid == null)
            {
                return HttpNotFound();
            }
            else
            {
                EstimationRequest er = (from e in db.EstimationRequests
                                        where e.Id == reqid
                                        select e).FirstOrDefault();

                TempData["ValuerRemark"] = er.ValuerRemark == null ? "" : er.ValuerRemark;
                TempData["requestid"] = er.Id;
                int[] stats = new int[] { 3, 4, 5 };
                ViewBag.Status = new SelectList(db.luDocumentStatus.Where(s=>stats.Contains(s.Id)).OrderBy(b => b.Id), "Id", "Status", er.Status);

                return View();
            }

        }
        [HttpPost]
        public JsonResult Evaluate()
        {
            if (Request["requestid"] != "" && Request["requestid"] != null && Request["ValuerRemark"] != "" && Request["ValuerRemark"] != null)
            {

                //string name = Convert.ToString(Session["LogedUserID"]);


                try
                {

                    int requestid = int.Parse(Request["requestid"]);
                    EstimationRequest er = (from e in db.EstimationRequests
                                            where e.Id == requestid
                                            select e).FirstOrDefault();
                    string rem = Request["ValuerRemark"];
                    rem = rem + "";
                    er.ValuerRemark = Request["ValuerRemark"];
                    er.Status = int.Parse(Request["Status"]);
                    db.Entry(er).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch
                {
                    return Json("Error Occured while processing your request. Please try again.");

                }

                return Json("Successfully registered valuer response");

            }



            return Json("Error Occured while processing your request. Please try again.");




        }
        public ActionResult HardCopyStatus(int? reqid)
        {

            if (reqid == null)
            {
                return HttpNotFound();
            }
            else
            {
                EstimationRequest er = (from e in db.EstimationRequests
                                        where e.Id == reqid
                                        select e).FirstOrDefault();

                TempData["HardCopyStatus"] = er.HardCopyStatus;
                TempData["requestid"] = er.Id;
                return View();
            }

        }
        [HttpPost]
         public JsonResult HardCopyStatus()
        {
            if (Request["requestid"] != "" && Request["requestid"] != null && Request["HardCopyStatus"] != "" && Request["HardCopyStatus"] != null)
            {

                //string name = Convert.ToString(Session["LogedUserID"]);


                try
                {

                    int requestid = int.Parse(Request["requestid"]);
                    EstimationRequest er = (from e in db.EstimationRequests
                                            where e.Id == requestid
                                            select e).FirstOrDefault();
                    er.HardCopyStatus = Request["HardCopyStatus"];
                    
                    db.Entry(er).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch
                {
                    return Json("Error Occured while processing your request. Please try again.");

                }

                return Json("Successfully updated hardcopy status ", JsonRequestBehavior.AllowGet);

            }



            return Json("Error Occured while processing your request. Please try again.");




        }
        
    }
}