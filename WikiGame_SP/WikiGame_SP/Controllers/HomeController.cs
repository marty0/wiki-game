﻿using System.Linq;
using System.Web.Mvc;
using WikiGame.Models;
using System.Web.Security;
using WikiGame_SP.Models;

namespace WikiGame.Controllers
{
    public class HomeController : Controller
    {
        private ICategoryProvider categoryProvider;

        public HomeController(ICategoryProvider categoryProvider)
        {
            this.categoryProvider = categoryProvider;
        }

        public ActionResult Index()
        {
            MembershipUser user = Membership.GetUser(false);
            var db = new Entities();

            if(user != null){
                ViewBag.loggedIn = true;
            }
            else{

                ViewBag.loggedIn = false;
            }
            var ip = Request.UserHostAddress;

            ViewBag.Categories = categoryProvider.GetAllCategories().Select(x => new SelectListItem() { Text = x.Name, Value = x.Name });

            var points = (from u in Membership.GetAllUsers().Cast<MembershipUser>()
                         join p in db.Points on u.ProviderUserKey.ToString() equals p.userId
                         group p by u.UserName into g
                         orderby g.Sum(x => x.points) descending
                         select new ScoreboardRecord
                         {
                             points = g.Sum(x => x.points),
                             userName = g.Key
                         }).AsEnumerable();

            return View(points);
        }

    }
}
