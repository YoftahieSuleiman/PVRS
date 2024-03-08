using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using University.Models;

namespace University.Controllers
{
    public class ReportsController : Controller
    {
        /*
        // GET: Reports
        //public static string strConn = ConfigurationManager.ConnectionStrings["FlexConnection"].ConnectionString;
        private pesEntities db = new pesEntities();

        public ActionResult CashBookBalance()
        {
          
           
            
            
            ViewBag.Denominations = db.luNotes.OrderBy(n => n.Currency).ThenBy(n => n.Note).ToList();
            var collecteds = db.DenominationDelivereds.Where(d => d.CollectionId != null).Where(d => d.CashCollection.DateCollected == DateTime.Today).OrderBy(n => n.CashCollection.Currency).ThenBy(n => n.luNote.Note).ToList();
            var delivereds = db.DenominationDelivereds.Where(d => d.RequestId != null).Where(d => d.CashRequest.DateResponded == DateTime.Today).OrderBy(n => n.CashRequest.Currency).ThenBy(n => n.luNote.Note).ToList();
            var totalcollecteds = db.DenominationDelivereds.Where(d => d.CollectionId != null).Where(d => d.CashCollection.DateCollected < DateTime.Today).OrderBy(n => n.CashCollection.Currency).ThenBy(n => n.luNote.Note).ToList();
            var totaldelivereds = db.DenominationDelivereds.Where(d => d.RequestId != null).Where(d => d.CashRequest.DateResponded < DateTime.Today).OrderBy(n => n.CashRequest.Currency).ThenBy(n => n.luNote.Note).ToList();
            var cashbooks = db.CashBooks.OrderBy(n => n.luNote.Currency).ThenBy(n => n.luNote.Note).ToList();
            ViewBag.DenominationCollecteds = collecteds;
            ViewBag.DenominationDelivereds = delivereds;
            ViewBag.TotalDenominationCollecteds = totalcollecteds;
            ViewBag.TotalDenominationDelivereds = totaldelivereds;
            ViewBag.cashbooks = cashbooks;
            List<string> currencies = new List<string>();
            foreach(DenominationDelivered dd in delivereds)
            {
                if (!currencies.Contains(dd.luNote.Currency))
                {
                    currencies.Add(dd.luNote.Currency);
                }
            }
            foreach (DenominationDelivered dd in collecteds)
            {
                if (!currencies.Contains(dd.luNote.Currency))
                {
                    currencies.Add(dd.luNote.Currency);
                }
            }
            ViewBag.currencies = currencies;
            return View();
        }
        public ActionResult CashCollection()
        {
          
           
            ViewBag.Branch = new SelectList(db.luOperationalLimits.OrderBy(n => n.Branch_Name), "Branch_code", "Branch_Name");
            ViewBag.BranchNames = (from e in db.luOperationalLimits
                                orderby e.Branch_Name
                                select e
                               ).ToList();
            
            ViewBag.Denominations = db.luNotes.OrderBy(n => n.Currency).ThenBy(n => n.Note).ToList();            
            var delivereds  = db.DenominationDelivereds.Where(d=>d.CollectionId != null).Where(d=>d.CashCollection.DateCollected==DateTime.Today).OrderBy(n => n.CashCollection.Currency).ThenBy(n => n.luNote.Note).ToList();
            ViewBag.DenominationDelivereds = delivereds;
            List<string> currencies = new List<string>();
            foreach(DenominationDelivered dd in delivereds)
            {
                if (!currencies.Contains(dd.luNote.Currency))
                {
                    currencies.Add(dd.luNote.Currency);
                }
            }
            ViewBag.currencies = currencies;
            return View();
        }

        public ActionResult CashCollectionFiltered(string[] branch,string from,string to)
        {
            if (branch.Count() == 1 && branch[0] == "")
            {
                ViewBag.Branch = new SelectList(db.luOperationalLimits.OrderBy(n => n.Branch_Name), "Branch_code", "Branch_Name");
                ViewBag.BranchNames = db.luOperationalLimits.OrderBy(n => n.Branch_Name).ToList();
            }
            else
            {
                ViewBag.Branch = new SelectList(db.luOperationalLimits.Where(o => branch.Contains(o.Branch_code)).OrderBy(n => n.Branch_Name), "Branch_code", "Branch_Name", branch);
                ViewBag.BranchNames = db.luOperationalLimits.Where(o => branch.Contains(o.Branch_code)).OrderBy(n => n.Branch_Name).ToList();
            }
           
            DateTime fromm = DateTime.Parse(from);
            DateTime too = DateTime.Parse(to);
            ViewBag.Denominations = db.luNotes.OrderBy(n => n.Currency).ThenBy(n => n.Note).ToList();
            var delivereds = db.DenominationDelivereds.Where(d => d.CollectionId != null).Where(d => d.CashCollection.DateCollected >= fromm).Where(d => d.CashCollection.DateCollected <= too).OrderBy(n => n.CashCollection.Currency).ThenBy(n => n.luNote.Note).ToList();
            
            ViewBag.DenominationDelivereds = delivereds;
            List<string> currencies = new List<string>();
            foreach (DenominationDelivered dd in delivereds)
            {
                if (!currencies.Contains(dd.luNote.Currency))
                {
                    currencies.Add(dd.luNote.Currency);
                }
            }
            ViewBag.currencies = currencies;
            return View();
        }
        public ActionResult CashDelivery()
        {
            ViewBag.Branch = new SelectList(db.luOperationalLimits.OrderBy(n => n.Branch_Name), "Branch_code", "Branch_Name");
            ViewBag.BranchNames = (from e in db.luOperationalLimits
                                   orderby e.Branch_Name
                                   select e
                               ).ToList();

            ViewBag.Denominations = db.luNotes.OrderBy(n => n.Currency).ThenBy(n => n.Note).ToList();
            var delivereds = db.DenominationDelivereds.Where(d => d.RequestId != null).Where(d => d.CashRequest.DateResponded == DateTime.Today).OrderBy(n => n.CashRequest.Currency).ThenBy(n => n.luNote.Note).ToList();
            ViewBag.DenominationDelivereds = delivereds;
            List<string> currencies = new List<string>();
            foreach (DenominationDelivered dd in delivereds)
            {
                if (!currencies.Contains(dd.luNote.Currency))
                {
                    currencies.Add(dd.luNote.Currency);
                }
            }
            ViewBag.currencies = currencies;
            return View();
        }
        public ActionResult CashDeliveryFiltered(string[] branch, string from, string to)
        {
            if (branch.Count() == 1 && branch[0] == "")
            {
                ViewBag.Branch = new SelectList(db.luOperationalLimits.OrderBy(n => n.Branch_Name), "Branch_code", "Branch_Name");
                ViewBag.BranchNames = db.luOperationalLimits.OrderBy(n => n.Branch_Name).ToList();
            }
            else
            {
                ViewBag.Branch = new SelectList(db.luOperationalLimits.Where(o => branch.Contains(o.Branch_code)).OrderBy(n => n.Branch_Name), "Branch_code", "Branch_Name", branch);
                ViewBag.BranchNames = db.luOperationalLimits.Where(o => branch.Contains(o.Branch_code)).OrderBy(n => n.Branch_Name).ToList();
            }

            DateTime fromm = DateTime.Parse(from);
            DateTime too = DateTime.Parse(to);
            ViewBag.Denominations = db.luNotes.OrderBy(n => n.Currency).ThenBy(n => n.Note).ToList();
            var delivereds = db.DenominationDelivereds.Where(d => d.RequestId != null).Where(d => d.CashRequest.DateResponded >= fromm).Where(d => d.CashRequest.DateResponded <= too).OrderBy(n => n.CashRequest.Currency).ThenBy(n => n.luNote.Note).ToList();

            ViewBag.DenominationDelivereds = delivereds;
            List<string> currencies = new List<string>();
            foreach (DenominationDelivered dd in delivereds)
            {
                if (!currencies.Contains(dd.luNote.Currency))
                {
                    currencies.Add(dd.luNote.Currency);
                }
            }
            ViewBag.currencies = currencies;
            return View();
        }


        public ActionResult SignableDocument()
        {
            var collection = 
  db.CashCollections
    .Where(s => s.Status == 5)
    .OrderByDescending(n => n.DateCollected)
    .Select(s => new 
     { 
       Id = s.Id,
       Description =(from b in db.luOperationalLimits where b.Branch_code==s.Branch select b.Branch_Name).FirstOrDefault()+", "+s.DateCollected.Value
     })
    .ToList();

            ViewBag.Collections = new SelectList(collection, "Id", "Description");


            var request =
 db.CashRequests
   .Where(s => s.Status == 5)
   .OrderByDescending(n => n.DateResponded)
   .Select(s => new
   {
       Id = s.Id,
       Description = (from b in db.luOperationalLimits where b.Branch_code == s.Branch select b.Branch_Name).FirstOrDefault() +", "+ s.DateResponded.Value
   })
   .ToList();

            ViewBag.Requests = new SelectList(request, "Id", "Description");

            return View();
        }
        public ActionResult SignableDocumentFiltered(string collectionId, string requestId)
        {
            ViewBag.BankingCenter = "";
            ViewBag.BranchManager = "";

            
            //Branch.Add("XML_File", "Settings.xml");



            if (collectionId != "")
            {
                int id = int.Parse(collectionId);
                var collection = (from c in db.CashCollections where c.Id == id select c).FirstOrDefault();
                ViewBag.Collection =collection;
                ViewBag.BankingCenter = (from b in db.luOperationalLimits where b.Branch_code == collection.Branch select b.Branch_Name).FirstOrDefault();
                ViewBag.Denominations = (from d in db.DenominationDelivereds where d.CollectionId == id select d).ToList();
                ViewBag.CurrencyName = (from c in db.luNotes where c.Currency == collection.Currency select c.Description).FirstOrDefault();
                


            }
            else if (requestId != "")
            {
                int id = int.Parse(requestId);
                var request = (from r in db.CashRequests where r.Id == id select r).FirstOrDefault();
                ViewBag.Request = request;
                ViewBag.BankingCenter = (from b in db.luOperationalLimits where b.Branch_code == request.Branch select b.Branch_Name).FirstOrDefault();
                ViewBag.Denominations = (from d in db.DenominationDelivereds where d.RequestId == id select d).ToList();
                ViewBag.CurrencyName = (from c in db.luNotes where c.Currency == request.Currency select c.Description).FirstOrDefault();
            }

            //***************Branch manager
            HRISDbHandle dbConnection = new HRISDbHandle();
            dbConnection.Open();
            DataTable userlocation = dbConnection.GetTable(@"SELECT  c.[Name]+' '+c.FName+' '+c.GFName as Full_Name FROM [HRIS].[dbo].[EmployeeExperience] a 
  join [HRIS].[dbo].luBankingCenter b on a.BankingCenter=b.Id 
  join [HRIS].[dbo].EmployeeDetail c on a.UserId=c.UserId
  where a.[Position]=2052
    and a.[To] is null
  and b.[Name] like '" + @ViewBag.BankingCenter + @"'");
            foreach (DataRow row in userlocation.Rows)
            {
                ViewBag.BranchManager = row["Full_Name"].ToString().Trim();
            }
            dbConnection.Close();
            //***************End Branch manager
            return View();
        }

        public ActionResult TicketSummary()
        {
            List<string> currencies = new List<string>();
            foreach (luNote n in db.luNotes.OrderBy(c=>c.Currency).ToList())
            {
                if (!currencies.Contains(n.Currency))
                {
                    currencies.Add(n.Currency);
                }
            }
            currencies.Remove("ETB");
            ViewBag.Currencies = new SelectList(db.luNotes.Where(l => l.Currency != "ETB").GroupBy(l => l.Currency)
                   .Select(grp => grp.FirstOrDefault()).OrderBy(c => c.Currency), "Currency", "Currency");
            //ViewBag.Currencies = db.luNotes.Where(l=>l.Currency!="ETB").Distinct().OrderBy(c => c.Currency);

            return View();
        }
        public ActionResult TicketSummaryFiltered(string type1, string type2,string currency ,string date)
        {
            DateTime reportdate = DateTime.Parse(date);
            ViewBag.Date=reportdate.ToString("MMMM dd,yyyy");
            ViewBag.Type = type2;
            ViewBag.CurrencyName = (from c in db.luNotes where c.Currency == currency select c.Description).FirstOrDefault();
            Dictionary<string, string> Branch = new Dictionary<string, string>();
            foreach (luOperationalLimit l in db.luOperationalLimits.OrderBy(o => o.Branch_Name))
            {
                Branch.Add(l.Branch_code, l.Branch_Name);
            }
            ViewBag.Branch = Branch;
            if (type1=="Collection" && type2=="FCY")
            {
                IEnumerable<CashCollection> collection = (from c in db.CashCollections where c.Status==5 where c.DateCollected == reportdate where c.Currency == currency select c).ToList();
                ViewBag.Collection = collection;
            }
            else if (type1 == "Collection" && type2 == "LCY")
            {
                IEnumerable<CashCollection> collection = (from c in db.CashCollections where c.Status == 5 where c.DateCollected == reportdate where c.Currency == "ETB" select c).ToList();
                ViewBag.Collection = collection;
              
            }
            else if (type1 == "Supply" && type2 == "FCY")
            {
                IEnumerable<CashRequest> supply = (from r in db.CashRequests where r.Status == 5 where r.DateResponded == reportdate where r.Currency == currency select r).ToList();
                ViewBag.Supply = supply;
               
            }
            else if (type1 == "Supply" && type2 == "LCY")
            {
                IEnumerable<CashRequest> supply = (from r in db.CashRequests where r.Status == 5 where r.DateResponded == reportdate where r.Currency == "ETB" select r).ToList();
                ViewBag.Supply = supply;
            }
            return View();
        }
        */
        pesEntities db = new pesEntities();
        public static string strConn = ConfigurationManager.ConnectionStrings["FlexConnection"].ConnectionString;

        public ActionResult ValuationRequest()
        {
            
            ViewBag.CustomerType = new SelectList(db.luCustomerTypes.OrderBy(n => n.Type), "Id", "Type");
            ViewBag.PropertyType = new SelectList(db.luPropertyTypes.OrderBy(n => n.Type), "Id", "Type");
            ViewBag.ValuationPurpose = new SelectList(db.luValuationPurposes.OrderBy(n => n.Purpose), "Id", "Purpose");
            ViewBag.AssignedTo = new SelectList(db.Users.Where(u => u.UserGroup == 2).OrderBy(n => n.UserName), "UserId", "UserName");
            ViewBag.Status = new SelectList(db.luDocumentStatus.OrderBy(n => n.Status), "Id", "Status");
            List<string> br3 = new List<string>();
            List<string> address = new List<string>() {"Addis Ababa","Afar Region","Amhara Region","Benishangul-Gumuz Region",
                "Central Ethiopia Regional State","Dire Dawa","Gambela Region","Harari Region","Oromia Region","Sidama Region","Somali Region",
                "South Ethiopia Regional State","South West Ethiopia Peoples Region","Tigray Region"
            };
            
                   
            using (OleDbConnection connection = new OleDbConnection(strConn))
            {
                string selectbranchsql = "select branch_name from fcubsprd.sttm_branch order by branch_name";
                OleDbCommand command = new OleDbCommand(selectbranchsql);

                // Set the Connection to the new OleDbConnection.
                command.Connection = connection;
                // Open the connection and execute the insert command.
                try
                {
                    connection.Open();
                    OleDbDataReader reader = command.ExecuteReader();
                    DropDownList dr = new DropDownList();
                    List<SelectListItem> br = new List<SelectListItem>();

                    while (reader.Read())
                    {
                        br3.Add(reader["branch_name"].ToString());
                        
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }

            List<EstimationRequest> er = (from e in db.EstimationRequests
                                          where e.Status != 7
                                          orderby e.DateUploaded descending
                                          select e).ToList();
            ViewBag.Branch = new SelectList((from s in br3.OrderBy(e => e).ToList()
                                                  select new
                                                  {
                                                      Id = s,
                                                      Description = s
                                                  }),
"Id",
"Description",
"");
            ViewBag.Address = new SelectList((from t in address.OrderBy(e => e).ToList()
                                             select new
                                             {
                                                 Id = t,
                                                 Description = t
                                             }),
      "Id",
      "Description",
      "");
            return View(er);
        }
        public ActionResult ValuationRequestFiltered(string[] Branch, int[] CustomerType, int[] PropertyType, int[] ValuationPurpose, int[] AssignedTo, int[] Status, string[] Address, string From, string To)
        {


            IEnumerable<EstimationRequest> er = (from e in db.EstimationRequests orderby e.DateUploaded select e).ToList();
            if (Branch[0] != "")
            {
                er = er.Where(o => Branch.Contains(o.RequestingBranch)).ToList();
            }
            if (CustomerType[0] > 0)
            {
                er = er.Where(o => CustomerType.Contains(o.CustomerType)).ToList();
            }
            if (PropertyType[0] > 0)
            {
                er = er.Where(o => PropertyType.Contains(o.PropertyType)).ToList();
            }
            if (ValuationPurpose[0] > 0)
            {
                er = er.Where(o => ValuationPurpose.Contains(o.ValuationPurpose)).ToList();
            }

            if (AssignedTo[0] > 0)
            {
                er = er.Where(o => AssignedTo.Contains(o.AssignedTo.Value)).ToList();
            }
            if (Status[0] > 0)
            {
                er = er.Where(o => Status.Contains(o.Status)).ToList();
            }
            if (Address[0] != "")
            {
                er = er.Where(o => Address.Contains(o.EstimationAddress1)).ToList();
            }
            if (From != "")
            {
                DateTime from = DateTime.Parse(From);
                er = er.Where(o => o.DateUploaded >= from).ToList();
            }
            if (To != "")
            {
                DateTime to = DateTime.Parse(To);
                er = er.Where(o => o.DateUploaded <= to).ToList();
            }
            
            return View(er);
        }
    }
}