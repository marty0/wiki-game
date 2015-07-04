﻿using System;
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
            if (!string.IsNullOrWhiteSpace(gameId) && GameHub.Games.ContainsKey(gameId) && !string.IsNullOrWhiteSpace(GameHub.Games[gameId].StartPage))
            {
                    @ViewBag.hasWon = false;
                    @ViewBag.wiki_page = new HtmlString(GameHub.Games[gameId].StartPage);
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
                GameHub.Games[gameId].StartPage = page;
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
                    var point = new Point();
                    point.userId = user.ProviderUserKey.ToString();
                    point.points = points;
                    point.timeElapsed = (int) time;
                    point.category = userInfo.CategoryName;
                    point.dateOfGame = DateTime.Now;
                    ViewBag.position = db.Points.Count(p => p.points <= points) + 1;
                    db.Points.Add(point);

                    db.SaveChanges();
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
