using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace WikiGame.Helpers
{
    public static class CustomHelpers
    {
        private static string virtualDirectory = VirtualPathUtility.ToAbsolute("~/");
        private static string cssDir = virtualDirectory + "Content/";
        private static string imageDir = virtualDirectory + "Content/images";
        private static string jsDir = virtualDirectory + "Scripts/";

        #region private methods

        /// <summary>
        /// Function that checks if the filename starts with "/" and 
        /// if it doesn't - it adds it.
        /// Script, CSS and Image use it.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>string</returns>

        private static void InsertSlashTo(ref string fileName)
        {
            if (!fileName.StartsWith("/"))
                fileName = fileName.Insert(0, "/");

            //return fileName;
        }

        #endregion

        /// <summary>
        /// HTML helper that generates js script tag for a given file name. 
        /// It is not necessary for the file name to ends with ".js"
        /// This method searches in ~/Scripts/js
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="fileName"></param>
        /// <returns>HtmlString</returns>
        public static HtmlString Script(this HtmlHelper helper, string fileName)
        {
            InsertSlashTo(ref fileName);


            if (!fileName.EndsWith(".js"))
                fileName += ".js";

            var jsPath = string.Format("<script src='{0}{1}' type='text/javascript'></script>\n", jsDir, helper.AttributeEncode(fileName));
            return new HtmlString(jsPath);
        }

        /// <summary>
        /// HTML helper that generates css tag for a given file name. 
        /// It is not necessary for the file name to ends with ".css"
        /// This method searches in ~/Content/Styles
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="fileName"></param>
        /// <returns>HtmlString</returns>
        public static HtmlString CSS(this HtmlHelper helper, string fileName)
        {
            InsertSlashTo(ref fileName);

            if (!fileName.EndsWith(".css"))
                fileName += ".css";
            var jsPath = string.Format("<link rel='stylesheet' type='text/css' href='{0}{1}'/>\n", cssDir, helper.AttributeEncode(fileName));
            return new HtmlString(jsPath);
        }

        /// <summary>
        /// HTML helper that generates image tag for a given file name. 
        /// This method searches in ~/Content/images
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="fileName"></param>
        /// <returns>HtmlString</returns>
        public static HtmlString Image(this HtmlHelper helper, string fileName)
        {
            return Image(helper, fileName, "");
        }

        /// <summary>
        /// HTML helper that generates image tag for a given file name and attributes.  
        /// This method searches in ~/Content/images
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="fileName"></param>
        /// <param name="attributes"></param>
        /// <returns>HtmlString</returns>
        public static HtmlString Image(this HtmlHelper helper, string fileName, string attributes)
        {
            InsertSlashTo(ref fileName);
            fileName = string.Format("{0}{1}", imageDir, fileName);
            return new HtmlString(string.Format("<img src='{0}' {1} />", helper.AttributeEncode(fileName), attributes));
        }

        /// <summary>
        /// Html helper that creates an image tag and accepts params in a Dictionary like way.
        /// </summary>
        /// <param name="helper">Extension method to HtmlHelper</param>
        /// <param name="fileName">Image filename used for src</param>
        /// <param name="htmlAttributes">Dictionary with attributes</param>
        /// <returns>Html span element</returns>
        public static HtmlString Image(this HtmlHelper helper, String fileName, Object htmlAttributes)
        {
            InsertSlashTo(ref fileName);
            fileName = string.Format("{0}{1}", imageDir, fileName);

            TagBuilder tagBuilder = new TagBuilder("img");
            tagBuilder.Attributes.Add("src", fileName);
            tagBuilder.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            return new HtmlString(tagBuilder.ToString(TagRenderMode.Normal));

        }

        /// <summary>
        /// Html helper for span tag element
        /// </summary>
        /// <param name="helper">Extension method to HtmlHelper</param>
        /// <param name="innerText">Text for the span</param>
        /// <param name="htmlAttributes">Dictionary with attributes</param>
        /// <returns>Html span element</returns>
        public static HtmlString Span(this HtmlHelper helper, string innerText, object htmlAttributes)
        {
            if (String.IsNullOrEmpty(innerText))
            {
                return new HtmlString("");
            }

            TagBuilder tagBuilder = new TagBuilder("span");
            tagBuilder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            tagBuilder.SetInnerText(innerText);

            return new HtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }
    }
}