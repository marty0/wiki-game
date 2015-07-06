using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using WikiGame.Utilities;
using WikiGame.Models;
using System.Web.Security;
using WikiGame_SP.Models;
using WikiGame_SP.Hubs;
using System.Configuration;

namespace WikiGame.Controllers
{
    public class GameWindowController : Controller
    {
        private static readonly string WIKI_BASE = "http://en.wikipedia.org/wiki/";
        private WikiPageParser wikiPageParser;
        private static Object thisLock = new Object();

        public GameWindowController(ICategoryProvider catProvider)
        {
            wikiPageParser = new WikiPageParser(catProvider);
        }

        public ActionResult Index(string categoryID, string gameId)
        {
            var userInfo = new UserInformation();
            userInfo.CategoryName = categoryID;
            System.Web.HttpContext.Current.Session["UserInfo"] = userInfo;
            System.Web.HttpContext.Current.Session["moves"] = 0;
            System.Web.HttpContext.Current.Session["time"] = DateTime.Now;

            return NewGame(gameId);
        }

        public ActionResult NewGame(string gameId)
        {
            lock (thisLock)
            {
                if (!string.IsNullOrWhiteSpace(gameId) && GameHub.Games.ContainsKey(gameId) && !string.IsNullOrWhiteSpace(GameHub.Games[gameId].StartPage))
                {
                    @ViewBag.hasWon = false;
                    @ViewBag.wiki_page = new HtmlString(GameHub.Games[gameId].StartPage);
                    System.Web.HttpContext.Current.Session["multyplayerGameId"] = gameId;
                    return View("Game");

                }

                var streamResponse = GetPageStream(WIKI_BASE + "Special:Random");

                bool hasWon;

                var page = wikiPageParser.GetContent(streamResponse, "", out hasWon);

                while (hasWon)
                {
                    page = wikiPageParser.GetContent(streamResponse, "", out hasWon);
                }

                @ViewBag.hasWon = hasWon;
                @ViewBag.wiki_page = new HtmlString(page);

                if (!string.IsNullOrWhiteSpace(gameId) && GameHub.Games.ContainsKey(gameId) && string.IsNullOrWhiteSpace(GameHub.Games[gameId].StartPage))
                {
                    System.Web.HttpContext.Current.Session["multyplayerGameId"] = gameId;
                    GameHub.Games[gameId].StartPage = page;
                }
            }

            return View("Game");
        }

        public ActionResult WikiPage(string article)
        {
            var streamResponse = GetPageStream(WIKI_BASE + article);

            bool hasWon;
            var userInfo = (UserInformation)System.Web.HttpContext.Current.Session["UserInfo"];

            var page = wikiPageParser.GetContent(streamResponse, userInfo.CategoryName, out hasWon);

            @ViewBag.hasWon = hasWon;

            SetPoints(hasWon);

            @ViewBag.wiki_page = new HtmlString(page);

            return View("Game");
        }

        private Stream GetPageStream(string url)
        {
            var myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            myHttpWebRequest.UserAgent = ".NET Framework Test Client";
            var myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            var streamResponse = myHttpWebResponse.GetResponseStream();

            return streamResponse;
        }

        private void SetPoints(bool hasWon)
        {
            System.Web.HttpContext.Current.Session["moves"] = (int)System.Web.HttpContext.Current.Session["moves"] + 1;

            if (hasWon)
            {
                var timeElapsed = DateTime.Now - (DateTime)System.Web.HttpContext.Current.Session["time"];
                double time = timeElapsed.TotalSeconds;
                int points = (int)System.Web.HttpContext.Current.Session["moves"];

                var user = Membership.GetUser(false);
                var userInfo = (UserInformation)System.Web.HttpContext.Current.Session["UserInfo"];
                var db = new Entities();
                if (user != null)
                {
                    if (System.Web.HttpContext.Current.Session["multyplayerGameId"] != null)
                    {
                        try
                        {
                            string gameId = System.Web.HttpContext.Current.Session["multyplayerGameId"].ToString();
                            var multiplayerGame = (from g in db.MultiplayerGames where g.gameId == gameId select g).AsEnumerable();
                            var currentGame = multiplayerGame.ElementAt(0);
                            currentGame.startPage = GameHub.Games[gameId.ToString()].StartPage;
                            currentGame.points = points;
                            timeElapsed = DateTime.Now - (DateTime)currentGame.dateOfGame;
                            currentGame.timeElapsed = (int)(timeElapsed.TotalSeconds);
                            currentGame.winner = user.UserName;
                            var original = db.MultiplayerGames.Find(currentGame.Id);
                            //db.Entry(original).CurrentValues.SetValues(currentGame);
                            ViewBag.position = db.MultiplayerGames.Count(p => p.points <= points && p.category == currentGame.category) + 1;
                            db.SaveChanges();

                        }
                        catch (System.Data.Entity.Validation.DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                                        ve.PropertyName,
                                        eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                                        ve.ErrorMessage);
                                }
                            }
                            throw;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.InnerException);
                        }
                    }
                    else
                    {
                        var point = new Point();
                        point.userId = user.ProviderUserKey.ToString();
                        point.points = points;
                        point.timeElapsed = (int)time;
                        point.category = userInfo.CategoryName;
                        point.dateOfGame = DateTime.Now;
                        ViewBag.position = db.Points.Count(p => p.points <= points) + 1;
                        db.Points.Add(point);

                        db.SaveChanges();
                    }
                }
                else
                {
                    ViewBag.position = db.Points.Count(p => p.points <= points) + 1;
                }
                ViewBag.appId = ConfigurationManager.AppSettings["AppId"];
                ViewBag.moves = points;
            }
        }
    }
}
