using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Text;
using WikiGame.Utilities;
using WikiGame.Models;
using System.Web.Security;
using WikiGame_SP.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace WikiGame.Controllers
{
    public class GameWindowController : Controller
    {
        //private static readonly int MAX_BUFF = 1024;
        private static readonly string WIKI_BASE = "http://en.wikipedia.org/wiki/";
        private WikiPageParser wikiPageParser;

        public GameWindowController(ICategoryProvider catProvider)
        {
            wikiPageParser = new WikiPageParser(catProvider);
        }


        public ActionResult Index(string CategoryID)
        {
            UserInformation userInfo = new UserInformation();
            userInfo.CategoryName = CategoryID;
            System.Web.HttpContext.Current.Session["UserInfo"] =userInfo;
            System.Web.HttpContext.Current.Session["moves"] = 0;
            System.Web.HttpContext.Current.Session["time"] = DateTime.Now;
            //Utilities.LoggedUsersInformation.GetUserInfoFor(User.Identity.Name).CategoryName = CategoryID;
            return View();
        }

        public ActionResult NewGame()
        {
            var streamResponse = GetPageStream(WIKI_BASE + "Special:Random");

            bool hasWon = false;
            
            var page = wikiPageParser.GetContent(streamResponse, "", out hasWon);

            @ViewBag.hasWon = hasWon;
            @ViewBag.wiki_page = new HtmlString(page);

            return View("Game");
        }

        public ActionResult WikiPage(string article)
        {
            //logger.Info(Request.UserHostAddress);

            var streamResponse = GetPageStream(WIKI_BASE + article);

            bool hasWon = false;
            UserInformation userInfo = (UserInformation)System.Web.HttpContext.Current.Session["UserInfo"];
            System.Web.HttpContext.Current.Session["moves"] = (int)System.Web.HttpContext.Current.Session["moves"] + 1;
            
            var page = wikiPageParser.GetContent(streamResponse, userInfo.CategoryName, out hasWon);

            @ViewBag.hasWon = hasWon;
            if (hasWon)
            {
                TimeSpan timeElapsed = DateTime.Now - (DateTime)System.Web.HttpContext.Current.Session["time"];
                double time = timeElapsed.TotalSeconds;
                int points = (int)System.Web.HttpContext.Current.Session["moves"];
                //int points = (int)time * (int)System.Web.HttpContext.Current.Session["moves"];
                MembershipUser user = Membership.GetUser(false);
                if (user != null)
                {
                    Entities db = new Entities();
                    Point point = new Point();
                    point.userId = user.ProviderUserKey.ToString();
                    point.points = points;
                    //point = point;
                    ViewBag.position = db.Points.Count(p => p.points <= points) + 1;
                    db.Points.Add(point);
                    
                    db.SaveChanges();



                }

            }
            @ViewBag.wiki_page = new HtmlString(page);

            return View("Game");
        }


        private Stream GetPageStream(string url)
        {
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            myHttpWebRequest.UserAgent = ".NET Framework Test Client";
            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            Stream streamResponse = myHttpWebResponse.GetResponseStream();

            return streamResponse;
        }

    }
}
