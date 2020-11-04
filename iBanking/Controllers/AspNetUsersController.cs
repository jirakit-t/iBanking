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
    public class AspNetUsersController : Controller
    {
        private BankingSystemEntities db = new BankingSystemEntities();

        // GET: AspNetUsers
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
            List<AspNetUser> UserList = new List<AspNetUser>();
            if(user.RoleID == 1)
            {
                UserList.AddRange(db.AspNetUsers.ToList());
            }
            else
            {
                UserList.AddRange(db.AspNetUsers.Where(a => a.BranchID == user.BranchID).ToList());
            }
            var aspNetUsers = db.AspNetUsers.Include(a => a.AspNetUser1);
            return View(aspNetUsers.ToList());
        }

        // GET: AspNetUsers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // GET: AspNetUsers/Create
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
            ViewBag.ManagerID = new SelectList(db.AspNetUsers.Where(a => a.RoleID < 3 && a.BranchID == user.BranchID), "Id", "Name");
            ViewBag.BranchID = new SelectList(db.Branches.Where(a => a.ID > 1), "ID", "Name");
            ViewBag.RoleID = new SelectList(db.AspNetRoles.Where(a => a.Id > user.RoleID), "ID", "Name");
            return View();
        }

        // POST: AspNetUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AspNetUser aspNetUser)
        {
            var Currentuser = Session["CurrentUser"];
            if (Currentuser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Currentuser = Currentuser;
            var URL = Request.Url.AbsoluteUri;
            ViewBag.URL = URL;
            AspNetUser existinguser = db.AspNetUsers.FirstOrDefault(a => a.Name == aspNetUser.Name);
            if (existinguser != null)
            {
                ModelState.AddModelError("Name", "Already have in database!!!");
            }
            if (ModelState.IsValid)
            {
                aspNetUser.CreatedDate = DateTime.Now;
                aspNetUser.BranchID = aspNetUser.BranchID;
                aspNetUser.UserName = aspNetUser.Name;
                aspNetUser.Password = "12345";
                db.AspNetUsers.Add(aspNetUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            int userid = Convert.ToInt32(Currentuser);
            AspNetUser user = db.AspNetUsers.FirstOrDefault(a => a.Id == userid);
            ViewBag.UserRole = user.RoleID;
            ViewBag.ManagerID = new SelectList(db.AspNetUsers.Where(a => a.RoleID < 3 && a.BranchID == user.BranchID), "Id", "Name", aspNetUser.ManagerID);
            ViewBag.BranchID = new SelectList(db.Branches.Where(a => a.ID > 1), "ID", "Name", aspNetUser.BranchID);
            ViewBag.RoleID = new SelectList(db.AspNetRoles.Where(a => a.Id > user.RoleID), "ID", "Name", aspNetUser.RoleID);
            return View(aspNetUser);
        }

        // GET: AspNetUsers/Edit/5
        public ActionResult Edit(string id)
        {
            var Currentuser = Session["CurrentUser"];
            if (Currentuser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Currentuser = Currentuser;
            var URL = Request.Url.AbsoluteUri;
            ViewBag.URL = URL;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int UserID = Convert.ToInt32(id);
            AspNetUser aspNetUser = db.AspNetUsers.Find(UserID);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            int userid = Convert.ToInt32(Currentuser);
            AspNetUser user = db.AspNetUsers.FirstOrDefault(a => a.Id == userid);
            ViewBag.UserRole = user.RoleID;
            ViewBag.ManagerID = new SelectList(db.AspNetUsers.Where(a => a.RoleID < 3 && a.BranchID == user.BranchID), "Id", "Name", aspNetUser.ManagerID);
            ViewBag.BranchID = new SelectList(db.Branches.Where(a => a.ID > 1), "ID", "Name", aspNetUser.BranchID);
            ViewBag.RoleID = new SelectList(db.AspNetRoles.Where(a => a.Id > user.RoleID), "ID", "Name", aspNetUser.RoleID);
            return View(aspNetUser);
        }

        // POST: AspNetUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AspNetUser aspNetUser)
        {
            var Currentuser = Session["CurrentUser"];
            if (Currentuser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Currentuser = Currentuser;
            var URL = Request.Url.AbsoluteUri;
            ViewBag.URL = URL;
            if (ModelState.IsValid)
            {
                db.Entry(aspNetUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            int userid = Convert.ToInt32(Currentuser);
            AspNetUser user = db.AspNetUsers.FirstOrDefault(a => a.Id == userid);
            ViewBag.UserRole = user.RoleID;
            ViewBag.ManagerID = new SelectList(db.AspNetUsers.Where(a => a.RoleID < 3 && a.BranchID == user.BranchID), "Id", "Name", aspNetUser.ManagerID);
            ViewBag.BranchID = new SelectList(db.Branches.Where(a => a.ID > 1), "ID", "Name", aspNetUser.BranchID);
            ViewBag.RoleID = new SelectList(db.AspNetRoles.Where(a => a.Id > user.RoleID), "ID", "Name", aspNetUser.RoleID);
            return View(aspNetUser);
        }

        // GET: AspNetUsers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: AspNetUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(aspNetUser);
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
