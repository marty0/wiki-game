using System.Linq;
using System.Web.Mvc;
using WikiGame.Models;
using System.Web.Security;
using WikiGame_SP.Models;
using System.Collections.Generic;

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
            System.DateTime lastMonth = System.DateTime.Now.AddMonths(-1);
            List<IEnumerable<ScoreboardRecord>> scoreboards = new List<IEnumerable<ScoreboardRecord>>();

            if(user != null){
                ViewBag.loggedIn = true;
            }
            else{

                ViewBag.loggedIn = false;
            }
            var ip = Request.UserHostAddress;

            ViewBag.Categories = categoryProvider.GetAllCategories().Select(x => new SelectListItem() { Text = x.Name, Value = x.Name });

            //var points = (from u in Membership.GetAllUsers().Cast<MembershipUser>()
            //             join p in db.Points on u.ProviderUserKey.ToString() equals p.userId
            //             group p by u.UserName into g
            //             orderby g.Sum(x => x.points) descending
            //             select new ScoreboardRecord
            //             {
            //                 points = g.Sum(x => x.points),
            //                 userName = g.Key
            //             }).AsEnumerable();
            //var points = (from game in db.MultiplayerGames
            //              group game by game.winner into g
            //              orderby g.Average(x => x.points.Value) ascending
            //              select new ScoreboardRecord
            //              {
            //                  points = g.Average(x => x.points.Value) + g.Average(x => x.timeElapsed),
            //                  userName = g.Key
            //              }).AsEnumerable();
            var points = (from game in db.MultiplayerGames
                          group game by game.winner into g
                          orderby g.Sum(x => (1000 - x.points + x.timeElapsed) > 0 ? (1000 - x.points + x.timeElapsed) : 0) descending
                          select new ScoreboardRecord
                          {
                              points = g.Sum(x => (1000 - x.points + x.timeElapsed) > 0 ? (1000 - x.points + x.timeElapsed) : 0),
                              userName = g.Key
                          }).AsEnumerable();
            scoreboards.Add(points);

            var lastMonthPoints = (from game in db.MultiplayerGames
                                    where game.dateOfGame > lastMonth
                                    group game by game.winner into g
                                    orderby g.Sum(x => (1000 - x.points + x.timeElapsed) > 0 ? (1000 - x.points + x.timeElapsed) : 0) descending
                                    select new ScoreboardRecord
                                    {
                                        points = g.Sum(x => (1000 - x.points + x.timeElapsed) > 0 ? (1000 - x.points + x.timeElapsed) : 0),
                                        userName = g.Key
                                    }).AsEnumerable();
            scoreboards.Add(lastMonthPoints);

            return View(scoreboards);
        }

    }
}
