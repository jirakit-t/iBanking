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
    public class BranchesController : Controller
    {
        private BankingSystemEntities db = new BankingSystemEntities();

        // GET: Branches
        public ActionResult Index()
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
            return View(db.Branches.ToList());
        }

        // GET: Branches/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        // GET: Branches/Create
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
            return View();
        }

        // POST: Branches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Address,Phone,Fax")] Branch branch)
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
            if (ModelState.IsValid)
            {
                db.Branches.Add(branch);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(branch);
        }

        // GET: Branches/Edit/5
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
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        // POST: Branches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Address,Phone,Fax")] Branch branch)
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
            if (ModelState.IsValid)
            {
                db.Entry(branch).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(branch);
        }

        // GET: Branches/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        // POST: Branches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Branch branch = db.Branches.Find(id);
            db.Branches.Remove(branch);
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
