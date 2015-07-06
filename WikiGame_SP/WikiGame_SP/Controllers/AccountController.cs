using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using WikiGame.Models;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using WikiGame_SP.Models;

namespace WikiGame.Controllers
{
    public class AccountController : Controller
    {

        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            ViewData["appId"] = ConfigurationManager.AppSettings["AppId"];
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/FBLogOn
        [HttpPost]
        public ActionResult FBLogOn(FBLogOnModel model)
        {
            if (ModelState.IsValid)
            {
                WebRequest request = WebRequest.Create("https://graph.facebook.com/oauth/access_token?client_id=" + ConfigurationManager.AppSettings["AppId"] +
                    "&client_secret=" + ConfigurationManager.AppSettings["ClientSecret"] +
                    "&grant_type=fb_exchange_token" +
                    "&%20fb_exchange_token=" + model.accessToken);

                using (WebResponse response = request.GetResponse())
                {

                    var responseBody = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    var longLivedAccessToken = responseBody.Split('&')[0];
                    longLivedAccessToken = longLivedAccessToken.Split('=')[1];
                    var client = new Facebook.FacebookClient(longLivedAccessToken);
                    dynamic me = client.Get("me?fields=first_name,last_name,id,email");

                    if (Membership.ValidateUser(me.email, longLivedAccessToken.Substring(0, 10)))
                    {
                        FormsAuthentication.SetAuthCookie(me.email, false /* createPersistentCookie */);
                        return RedirectToAction("Index", "Home");
                    }

                    // Attempt to register the user
                    MembershipCreateStatus createStatus;
                    //get only the first 10 chars from the token, because it is too long to be password
                    Membership.CreateUser(me.email, longLivedAccessToken.Substring(0, 10), me.email, null, null, true, null, out createStatus);

                    if (createStatus == MembershipCreateStatus.Success)
                    {
                        FormsAuthentication.SetAuthCookie(me.email, false /* createPersistentCookie */);
                        return RedirectToAction("Index", "Home");
                    }

                }
            }
            // If we got this far, something failed, redisplay form
            return RedirectToAction("LogOn", "Account");
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("LogOn", "Account");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public ActionResult RestorePassword()
        {
            return View();
        }

        //
        // POST: /Account/RestorePassword

        [HttpPost]
        public ActionResult RestorePassword(PasswordRestorerModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
               string username = Membership.GetUserNameByEmail(model.Email);
               if (username != null)
               {
                   var user = Membership.GetUser(username);
                   string newPass = user.ResetPassword();
                   try
                   {
                       MailMessage mailMessage = new MailMessage();
                       mailMessage.To.Add(model.Email);
                       mailMessage.From = new MailAddress("wikigamecustomerservice@gmail.com");
                       mailMessage.Subject = "Wiki game restore password";
                       mailMessage.IsBodyHtml = true;
                       mailMessage.Body = "Hello,</br> this is your new password: " + newPass + "</br> Have a great game at <a target=\"_blank\" href=\"http://localhost:1337/Wiki/\">WikiGame</a>!</br></br>Regards,</br> The Wiki Game Team";
                       SmtpClient smtp = new SmtpClient();
                       smtp.UseDefaultCredentials = true;
                       smtp.Port = 587;
                       smtp.EnableSsl = true;
                       smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["SMTPUser"], ConfigurationManager.AppSettings["SMTPPass"]);
                       //smtp.Credentials = new NetworkCredential("wikigamecustomerservice@gmail.com", "WikiGame&CustomerService");
                       smtp.Host = "smtp.gmail.com";
                       smtp.Send(mailMessage);
                       @ViewBag.email_sent = true;
                   }
                   catch (Exception ex)
                   {
                       //Response.Write("Could not send the e-mail - error: " + ex.Message);
                   }
               }
               else
               {
                   @ViewBag.noUser = "No user with such email was found!";
               }
            }
            return View(model);
        }

        public ActionResult Profile()
        {
            MembershipUser user = Membership.GetUser(false);
            var db = new Entities();
            if (user == null)
            {
                return RedirectToAction("LogOn", "Account");
            }
            else
            {
                var userId = user.ProviderUserKey.ToString();
                var points = (from p in db.Points
                              where p.userId == userId
                              orderby p.Id descending
                              select new ProfilePointsModel
                              {
                                  points = p.points,
                                  timeElapsed = p.timeElapsed,
                                  category = p.category,
                                  dateOfGame = p.dateOfGame

                              }).AsEnumerable();
                             
               /*var all = (from u in Membership.GetAllUsers().Cast<MembershipUser>()
                              join p in db.Points on u.ProviderUserKey.ToString() equals p.userId
                              group p by u.UserName into g
                              orderby g.Sum(x => x.points) descending
                              select new ScoreboardRecord
                              {
                                  points = g.Sum(x => x.points),
                                  userName = g.Key
                              }).AsEnumerable();
                var userPoints = (from o in all where o.userName == user.UserName select o.points);
                if (userPoints.Count() > 0)
                {
                    var medal = (from m in all
                                 where m.points < userPoints.ElementAt(0)
                                 select m).Count();
                    if (medal < 1)
                    {
                        ViewBag.medal = "gold";

                    }
                    else
                    {
                        if (medal < 2)
                        {
                            ViewBag.medal = "silver";

                        }
                        else
                        {
                            if (medal < 3)
                            {
                                ViewBag.medal = "bronze";

                            }

                        }
                    }
                }
                */
                var userName = user.UserName;
                var multiplayerPoints = (from p in db.MultiplayerGames
                              where (p.userId1 == userName || p.userId2 == userName) && p.winner != null
                              orderby p.Id descending
                              select new ProfileMultiplayerPoints
                              {
                                  points = (int) p.points,
                                  timeElapsed = p.timeElapsed,
                                  category = p.category,
                                  dateOfGame = p.dateOfGame,
                                  winner = (p.winner == userName ? "You Won!" : "You Lost!"),
                                  opponent = (p.userId1 == userName ? p.userId2 : p.userId1)

                              }).AsEnumerable();
                var allMultiplayer = (from game in db.MultiplayerGames
                              group game by game.winner into g
                              orderby g.Sum(x => (1000 - x.points + x.timeElapsed) > 0 ? (1000 - x.points + x.timeElapsed) : 0) descending
                              select new ScoreboardRecord
                              {
                                  points = g.Sum(x => (1000 - x.points + x.timeElapsed) > 0 ? (1000 - x.points + x.timeElapsed) : 0),
                                  userName = g.Key
                              }).AsEnumerable();
                /*var allMultiplayer = (from u in Membership.GetAllUsers().Cast<MembershipUser>()
                           join p in db.MultiplayerGames on u.UserName equals p.winner
                           group p by u.UserName into g
                           orderby g.Min(x => x.points) descending
                           select new ScoreboardRecord
                           {
                               points = g.Min(x => (int) x.points),
                               userName = g.Key
                           }).AsEnumerable();*/
                var userPointsMultiplayer = (from o in allMultiplayer where o.userName == user.UserName select o.points);
                if (userPointsMultiplayer.Count() > 0)
                {
                    var medal = (from m in allMultiplayer
                                 where m.points > userPointsMultiplayer.ElementAt(0)
                                 select m).Count();
                    if (medal < 1)
                    {
                        ViewBag.medalMultyplayer = "gold";

                    }
                    else
                    {
                        if (medal < 2)
                        {
                            ViewBag.medalMultyplayer = "silver";

                        }
                        else
                        {
                            if (medal < 3)
                            {
                                ViewBag.medalMultyplayer = "bronze";

                            }

                        }
                    }
                }
                ViewData["points"] = points;
                ViewData["multiplayerPoints"] = multiplayerPoints;
                return View();
            }
            
        }
        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
