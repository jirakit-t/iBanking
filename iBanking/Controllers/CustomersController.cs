using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using iBanking.Models;

namespace iBanking.Controllers
{
    public class CustomersController : Controller
    {
        private BankingSystemEntities db = new BankingSystemEntities();

        // GET: Customers
        public ActionResult Index()
        {
            var Currentuser = Session["CurrentUser"];
            if (Currentuser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Currentuser = Currentuser;
            var URL = Request.Url.AbsoluteUri;
            ViewBag.URL = URL;
            int userid = Convert.ToInt32(Currentuser);
            AspNetUser user = db.AspNetUsers.FirstOrDefault(a => a.Id == userid);
            ViewBag.UserRole = user.RoleID;
            List<Customer> CustomerList = new List<Customer>();
            if(user.BranchID != 1)
            {
                CustomerList.AddRange(db.Customers.Where(a => a.BranchID == user.BranchID).ToList());
            }
            else
            {
                CustomerList.AddRange(db.Customers.ToList());
            }
            return View(CustomerList);
        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            var Currentuser = Session["CurrentUser"];
            if (Currentuser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Currentuser = Currentuser;
            var URL = Request.Url.AbsoluteUri;
            ViewBag.URL = URL;
            int userid = Convert.ToInt32(Currentuser);
            AspNetUser user = db.AspNetUsers.FirstOrDefault(a => a.Id == userid);
            ViewBag.UserRole = user.RoleID;
            ViewBag.BranchID = new SelectList(db.Branches.Where(a => a.ID > 1), "ID", "Name");
            ViewBag.BirthDate = new DateTime(2000, 1, 1);

            var js = new MSScriptControl.ScriptControlClass
            {
                AllowUI = false,
                Language = "JScript"
            };
            js.Reset();

            string jscode = ReadResource();
            js.AddCode(jscode);
            object[] parms = new object[] { "Netherlands" };
            string result = (string)js.Run("buildIbans", ref parms);
            ViewBag.IbanNo = result;
            TempData["IbanNoCode"] = result;

            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,IBANNo,CardID,Name,BranchID,CreateDate,BirthDate,Email,Address,Phone1,Phone2")] Customer customer)
        {
            var Currentuser = Session["CurrentUser"];
            if (Currentuser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Currentuser = Currentuser;
            var URL = Request.Url.AbsoluteUri;
            ViewBag.URL = URL;
            int userid = Convert.ToInt32(Currentuser);
            AspNetUser user = db.AspNetUsers.FirstOrDefault(a => a.Id == userid);
            ViewBag.UserRole = user.RoleID;
            if (customer.CardID == null)
            {
                ModelState.AddModelError("CardID", "Please filled the CardID field");
            }
            if (customer.BirthDate == null)
            {
                ModelState.AddModelError("BirthDate", "Please filled the BirthDate field");
            }
            if (customer.Email == null)
            {
                ModelState.AddModelError("Email", "Please filled the Email field");
            }
            if (customer.Name == null)
            {
                ModelState.AddModelError("Name", "Please filled the Name field");
            }
            if (ModelState.IsValid)
            {
                customer.IBANNo = TempData["IbanNoCode"].ToString();
                customer.CreateDate = DateTime.Now;
                if(user.BranchID != 1)
                {
                    customer.BranchID = user.BranchID;
                }
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var js = new MSScriptControl.ScriptControlClass
            {
                AllowUI = false,
                Language = "JScript"
            };
            js.Reset();

            string jscode = ReadResource();
            js.AddCode(jscode);
            object[] parms = new object[] { "Netherlands" };
            string result = (string)js.Run("buildIbans", ref parms);
            ViewBag.IbanNo = result;
            TempData["IbanNoCode"] = result;

            ViewBag.UserRole = user.RoleID;
            ViewBag.BranchID = new SelectList(db.Branches.Where(a => a.ID > 1), "ID", "Name", customer.BranchID);
            return View(customer);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserRole = user.RoleID;
            ViewBag.BirthDate = customer.BirthDate;
            ViewBag.IbanNo = customer.IBANNo;
            ViewBag.BranchID = new SelectList(db.Branches.Where(a => a.ID > 1), "ID", "Name", customer.BranchID);
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,IBANNo,CardID,Name,BranchID,CreateDate,BirthDate,Email,Address,Phone1,Phone2")] Customer customer)
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
            if (customer.CardID == null)
            {
                ModelState.AddModelError("CardID", "Please filled the CardID field");
            }
            if (customer.BirthDate == null)
            {
                ModelState.AddModelError("BirthDate", "Please filled the BirthDate field");
            }
            if (customer.Email == null)
            {
                ModelState.AddModelError("Email", "Please filled the Email field");
            }
            if (customer.Name == null)
            {
                ModelState.AddModelError("Name", "Please filled the Name field");
            }
            if (ModelState.IsValid)
            {
                customer.IBANNo = customer.IBANNo;
                customer.CardID = customer.CardID;
                customer.BirthDate = customer.BirthDate;
                if (user.BranchID != 1)
                {
                    customer.BranchID = user.BranchID;
                }
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BranchID = new SelectList(db.Branches.Where(a => a.ID > 1), "ID", "Name", customer.BranchID);
            return View(customer);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private static string ReadResource()
        {
            string result;
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "iBanking.IbanGenerator.js";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }

            return result;
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
