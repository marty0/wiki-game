using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using System.IO;
using WikiGame.Models;

namespace WikiGame.Utilities
{
    public class WikiPageParser
    {
        private static readonly string PAGE_LINK = VirtualPathUtility.ToAbsolute("~") + "/GameWindow/WikiPage?article=";
        private ICategoryProvider catProvider;

        public WikiPageParser(Models.ICategoryProvider catProvider)
        {
            this.catProvider = catProvider;
        }

        public string GetContent(Stream page, string categoryName, out bool hasWon)
        {
            hasWon = false;

            HtmlDocument htmlDoc = new HtmlDocument();

            htmlDoc.OptionFixNestedTags = true;

            htmlDoc.Load(page);

            if (htmlDoc.DocumentNode != null)
            {
                HtmlNode bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='content']");

                if (bodyNode != null)
                {
                    if (String.IsNullOrEmpty(categoryName))
                    {
                        hasWon = false;
                    }
                    else
                    {
                        HtmlNode title = htmlDoc.DocumentNode.SelectNodes("//h1[@id='firstHeading']")[0];

                        hasWon = HasAWinningWord(title.InnerText, categoryName);
                    }


                    foreach (HtmlNode link in htmlDoc.DocumentNode.SelectNodes("//a[@href]"))
                    {
                        HtmlAttribute att = link.Attributes["href"];
                        if (att.Value.StartsWith("/wiki/"))
                            att.Value = PAGE_LINK + att.Value.Split('/')[2];
                        else
                            link.InnerHtml = "";
                    }
                }
                return bodyNode.InnerHtml;
            }

            return "";
        }


        private bool HasAWinningWord(string title, string categoryName)
        {
            bool hasWon = false;

            string[] titleWords = title.ToLower().Split(' ');

            var intersect = titleWords.Intersect(catProvider.GetCategoryByName(categoryName).KeyWords);

            if (intersect.Count() == 0)
                hasWon = false;
            else
                hasWon = true;

            return hasWon;
        }
    }
}