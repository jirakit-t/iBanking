using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using iBanking.Models;

namespace iBanking.Controllers
{
    public class TransitionsController : Controller
    {
        private BankingSystemEntities db = new BankingSystemEntities();

        // GET: Transitions
        public ActionResult Index()
        {
            var transitions = db.Transitions.Include(t => t.AspNetUser).Include(t => t.Category).Include(t => t.Customer).Include(t => t.Customer1);
            return View(transitions.ToList());
        }

        public ActionResult YearlyTransition()
        {
            var Currentuser = Session["CurrentUser"];
            if (Currentuser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int userid = Convert.ToInt32(Currentuser);
            AspNetUser user = db.AspNetUsers.FirstOrDefault(a => a.Id == userid);
            ViewBag.Currentuser = Currentuser;
            ViewBag.UserRole = user.RoleID;
            var URL = Request.Url.AbsoluteUri;
            ViewBag.URL = URL;
            List<TransitionChart> Transitionchart = new List<TransitionChart>();
            var tempdate = new DateTime(DateTime.Now.Year, 1, 1);
            while (tempdate.Year < DateTime.Now.AddYears(1).Year)
            {
                decimal deposit = 0;
                decimal withdraw = 0;
                decimal transfer = 0;
                decimal fee = 0;
                decimal balance = 0;
                TransitionChart data = new TransitionChart();
                data.Date = tempdate.Month;
                data.NumberAccount = db.Customers.Where(a => a.CreateDate.Year == tempdate.Year && a.CreateDate.Month == tempdate.Month).Count();
                data.NumberBanker = db.AspNetUsers.Where(a => a.CreatedDate.Value.Year == tempdate.Year && a.CreatedDate.Value.Month == tempdate.Month).Count();
                if (db.Transitions.Where(a => a.CreateDate.Year == tempdate.Year && a.CreateDate.Month == tempdate.Month && a.CategoryID == 1).Count() > 0)
                {
                    deposit = db.Transitions.Where(a => a.CreateDate.Year == tempdate.Year && a.CreateDate.Month == tempdate.Month && a.CategoryID == 1).Sum(a => a.AfterFee);
                    balance = balance + deposit;
                }
                if(db.Transitions.Where(a => a.CreateDate.Year == tempdate.Year && a.CreateDate.Month == tempdate.Month && a.CategoryID == 2).Count() > 0)
                {
                    withdraw = db.Transitions.Where(a => a.CreateDate.Year == tempdate.Year && a.CreateDate.Month == tempdate.Month && a.CategoryID == 2).Sum(a => a.AfterFee);
                    balance = balance - withdraw;
                }
                if(db.Transitions.Where(a => a.CreateDate.Year == tempdate.Year && a.CreateDate.Month == tempdate.Month && a.CategoryID == 3).Count() > 0)
                {
                    transfer = db.Transitions.Where(a => a.CreateDate.Year == tempdate.Year && a.CreateDate.Month == tempdate.Month && a.CategoryID == 3).Sum(a => a.AfterFee);
                    balance = balance + transfer;
                }
                if(db.Transitions.Where(a => a.CreateDate.Year == tempdate.Year && a.CreateDate.Month == tempdate.Month).Count() > 0)
                {
                    fee = db.Transitions.Where(a => a.CreateDate.Year == tempdate.Year && a.CreateDate.Month == tempdate.Month).Sum(a => a.Fee);
                }
                data.Deposit = deposit;
                data.Withdraw = withdraw;
                data.Transfer = transfer;
                data.Fee = fee;
                data.Balance = balance;

                Transitionchart.Add(data);
                tempdate = tempdate.AddMonths(1);
            }
            ViewBag.Transitionchart = Transitionchart;
            return View();
        }

        // GET: Transitions/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transition transition = db.Transitions.Find(id);
            if (transition == null)
            {
                return HttpNotFound();
            }
            return View(transition);
        }

        // GET: Transitions/Create
        public ActionResult Create(int CategoryID)
        {
            var Currentuser = Session["CurrentUser"];
            if (Currentuser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var URL = Request.Url.AbsoluteUri;
            ViewBag.URL = URL;
            ViewBag.Currentuser = Currentuser;
            int userid = Convert.ToInt32(Currentuser);
            AspNetUser user = db.AspNetUsers.FirstOrDefault(a => a.Id == userid);
            ViewBag.UserRole = user.RoleID;
            ViewBag.CustomerID = new SelectList(db.Customers.Select(a => new { ID = a.ID, Value = a.IBANNo + " - " + a.Name }), "ID", "Value");
            ViewBag.ToCustomerID = new SelectList(db.Customers.Select(a => new { ID = a.ID, Value = a.IBANNo + " - " + a.Name }), "ID", "Value");
            ViewBag.BranchID = new SelectList(db.Branches.Where(a => a.ID > 1), "ID", "Name");
            ViewBag.CategoryID = CategoryID;
            return View();
        }
        
        public ActionResult FeeCalculate(decimal? Amount)
        {
            decimal? fee = Convert.ToDecimal(0.1);
            decimal? feevalue = (Amount * fee) / 100;
            decimal? netamount = Amount - feevalue;
            decimal? amount = Amount;
            var json = Json(new
            {
                Amount = amount,
                Fee = feevalue,
                NetAmount = netamount
            }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public ActionResult filterToCustomer(int? CustomerID)
        {
            var balance = db.CustomerBalance(CustomerID);
            var Customer = db.Customers.Select(a => new SelectListItem { Value = a.ID.ToString(), Text = a.IBANNo + " - " + a.Name });
            if (CustomerID != null)
            {
                Customer = db.Customers.Where(a => a.ID != CustomerID)
                                    .Select(a => new SelectListItem { Value = a.ID.ToString(), Text = a.IBANNo + " - " + a.Name });
            }
            SelectList CustomerFilteredData = new SelectList(Customer, "Value", "Text");
            var json = Json(new
            {
                customerfilterid = CustomerFilteredData,
                balance = balance
            }, JsonRequestBehavior.AllowGet);
            return json;
        }

        [HttpPost]
        public ActionResult GetAccountView(string search)
        {
            var Customer = db.Customers.Select(a => new SelectListItem { Value = a.ID.ToString(), Text = a.IBANNo + " - " + a.Name });
            if (search != "")
            {
                Customer = db.Customers.Where(a => a.IBANNo.Contains(search)).OrderBy(a => a.IBANNo)
                                    .Select(a => new SelectListItem { Value = a.ID.ToString(), Text = a.IBANNo + " - " + a.Name });
            }
            SelectList Accountlist = new SelectList(Customer, "Value", "Text");
            return Json(Accountlist);
        }

        [HttpPost]
        public ActionResult GetToAccountView(string search, int CustomerID)
        {
            var Customer = db.Customers.Select(a => new SelectListItem { Value = a.ID.ToString(), Text = a.IBANNo + " - " + a.Name });
            if (search != "")
            {
                Customer = db.Customers.Where(a => a.IBANNo.Contains(search) && a.ID != CustomerID).OrderBy(a => a.IBANNo)
                                    .Select(a => new SelectListItem { Value = a.ID.ToString(), Text = a.IBANNo + " - " + a.Name });
            }
            SelectList AccountTolist = new SelectList(Customer, "Value", "Text");
            return Json(AccountTolist);
        }

        // POST: Transitions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CreateDate,LastUpdated,CategoryID,CustomerID,ToCustomerID,Amount,Fee,AfterFee,BranchID,UserID,Note")] Transition transition)
        {
            var Currentuser = Session["CurrentUser"];
            if (Currentuser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Currentuser = Currentuser;
            int userid = Convert.ToInt32(Currentuser);
            AspNetUser user = db.AspNetUsers.FirstOrDefault(a => a.Id == userid);
            ViewBag.UserRole = user.RoleID;
            var URL = Request.Url.AbsoluteUri;
            ViewBag.URL = URL;
            decimal fee = Convert.ToDecimal(0.1);
            var BalanceCustomer = db.AllCustomerBalances.FirstOrDefault(a => a.ID == transition.CustomerID).Balance;
            if(transition.CategoryID != 1)
            {
                if (transition.Amount > BalanceCustomer)
                {
                    ModelState.AddModelError("Amount", "Please filled the Amount less than Balance field");
                }
            }
            if (ModelState.IsValid)
            {
                transition.CreateDate = DateTime.Now;
                transition.LastUpdated = DateTime.Now;
                if (user.BranchID != 1)
                {
                    transition.BranchID = user.BranchID;
                }
                transition.UserID = userid;
                if(transition.CategoryID != 1)
                {
                    transition.Fee = 0;
                    transition.AfterFee = transition.Amount;
                }
                else
                {
                    transition.Fee = (transition.Amount * fee) / 100;
                    transition.AfterFee = transition.Amount - transition.Fee;
                }
                db.Transitions.Add(transition);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            if(transition.CategoryID != 1)
            {
                ViewBag.AfterFee = transition.Amount;
            }
            ViewBag.UserRole = user.RoleID;
            ViewBag.CategoryID = transition.CategoryID;
            ViewBag.Balance = BalanceCustomer.Value.ToString("#,##0.##");
            ViewBag.BranchID = new SelectList(db.Branches.Where(a => a.ID > 1), "ID", "Name", transition.BranchID);
            ViewBag.CustomerID = new SelectList(db.Customers.Select(a => new { ID = a.ID, Value = a.IBANNo + " - " + a.Name }), "ID", "Value", transition.CustomerID);
            ViewBag.ToCustomerID = new SelectList(db.Customers.Select(a => new { ID = a.ID, Value = a.IBANNo + " - " + a.Name }), "ID", "Value", transition.ToCustomerID);
            return View(transition);
        }

        // GET: Transitions/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transition transition = db.Transitions.Find(id);
            if (transition == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", transition.UserID);
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", transition.CategoryID);
            ViewBag.CustomerID = new SelectList(db.Customers, "ID", "IBANNo", transition.CustomerID);
            ViewBag.ToCustomerID = new SelectList(db.Customers, "ID", "IBANNo", transition.ToCustomerID);
            return View(transition);
        }

        // POST: Transitions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CreateDate,LastUpdated,CategoryID,CustomerID,ToCustomerID,Amount,Fee,AfterFee,BranchID,UserID,Note")] Transition transition)
        {
            if (ModelState.IsValid)
            {
                transition.LastUpdated = DateTime.Now;
                db.Entry(transition).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", transition.UserID);
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", transition.CategoryID);
            ViewBag.CustomerID = new SelectList(db.Customers, "ID", "IBANNo", transition.CustomerID);
            ViewBag.ToCustomerID = new SelectList(db.Customers, "ID", "IBANNo", transition.ToCustomerID);
            return View(transition);
        }

        // GET: Transitions/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transition transition = db.Transitions.Find(id);
            if (transition == null)
            {
                return HttpNotFound();
            }
            return View(transition);
        }

        // POST: Transitions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Transition transition = db.Transitions.Find(id);
            db.Transitions.Remove(transition);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
