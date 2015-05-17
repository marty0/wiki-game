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
            var page = wikiPageParser.GetContent(streamResponse, userInfo.CategoryName, out hasWon);

            @ViewBag.hasWon = hasWon;
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
