using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WikiGame.Models;

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
            var ip = Request.UserHostAddress;

            ViewBag.Categories = categoryProvider.GetAllCategories().Select(x => new SelectListItem() { Text = x.Name, Value = x.Name });

            return View();
        }

    }
}
