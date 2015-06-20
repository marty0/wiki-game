using System.Linq;
using System.Web.Mvc;
using WikiGame.Models;
using System.Web.Security;

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
            if(user != null){
                ViewBag.loggedIn = true;
            }
            else{

                ViewBag.loggedIn = false;
            }
            var ip = Request.UserHostAddress;

            ViewBag.Categories = categoryProvider.GetAllCategories().Select(x => new SelectListItem() { Text = x.Name, Value = x.Name });

            return View();
        }

    }
}
