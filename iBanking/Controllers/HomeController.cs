using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iBanking.Models;

namespace iBanking.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private BankingSystemEntities db = new BankingSystemEntities();
        public ActionResult Index()
        {
            var Currentuser = Session["CurrentUser"];
            if(Currentuser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int userid = Convert.ToInt32(Currentuser);
            AspNetUser user = db.AspNetUsers.FirstOrDefault(a => a.Id == userid);
            ViewBag.Currentuser = Currentuser;
            ViewBag.UserRole = user.RoleID;
            ViewBag.ImagePath = "/medias/Users/Banker.png";
            var URL = Request.Url.AbsoluteUri;
            ViewBag.URL = URL;
            var today = DateTime.Now;
            var NumTransition = 0;
            decimal fee = 0;
            decimal Balance = 0;
            decimal Deposit = 0;
            decimal Withdraw = 0;
            decimal Transfer = 0;

            var TransitionList = db.Transitions.Where(a => a.CreateDate.Year == today.Year && a.CreateDate.Month == today.Month && a.CreateDate.Day == today.Day && a.UserID == userid).OrderByDescending(a => a.CreateDate).ToList();
            NumTransition = TransitionList.Count();
            fee = TransitionList.Sum(a => a.Fee);
            Deposit = TransitionList.Where(a => a.CategoryID == 1).Sum(a => a.AfterFee);
            Withdraw = TransitionList.Where(a => a.CategoryID == 2).Sum(a => a.AfterFee);
            Transfer = TransitionList.Where(a => a.CategoryID == 3).Sum(a => a.AfterFee);
            Balance = Deposit + Transfer - Withdraw;

            ViewBag.UserName = user.Name;
            ViewBag.TransitionList = TransitionList;
            ViewBag.TransitionToday = NumTransition;
            ViewBag.BalanceTransitionToday = Balance.ToString("#,##0.##");
            ViewBag.FeeTransitionToday = fee.ToString("#,##0.##");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}