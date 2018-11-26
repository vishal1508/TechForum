using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Tech_Forum.Models;
using System.IO;
using System.Web.Security;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using Newtonsoft.Json;

namespace Tech_Forum.Controllers
{
    public class SubscriberController : Controller
    {
        //Registration Action
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        //Registration POST Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified,ActivationCode")]Subscriber_Table subscriber)
        {
            bool Status = false;
            string message = "";
            StreamWriter stream = null;

            //Model Validation
            if (ModelState.IsValid)
            {
                #region Email is already existing
                var isExist = IsEmailExist(subscriber.email);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View(subscriber);
                }
                #endregion

                #region Activation Code Generation
                subscriber.ActivationCode = Guid.NewGuid();
                #endregion

                #region Password Hashing
                subscriber.password = Crypto.Hash(subscriber.password);
                subscriber.confirmpassword = Crypto.Hash(subscriber.confirmpassword);
                #endregion
                subscriber.IsEmailVerified = false;

                #region Save data to database
                using (PostEntity pe = new PostEntity())
                {
                    pe.Subscriber_Table.Add(subscriber);
                    try
                    {
                        pe.SaveChanges();
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        stream= new StreamWriter(@"D:\Exception.txt");
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                stream.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                            }
                        }
                        stream.Close();
                    }
                    finally
                    {
                        //stream.Close();
                    }


                    //Send Email to user
                    SendVerificationLinkEmail(subscriber.email, subscriber.ActivationCode.ToString());
                    message = "Registration successfully done. Account activation link has been sent to your emailid " + subscriber.email;

                    Status = true;

                }
                #endregion

            }
            else
            {
                message = "Invalid request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;


            return View(subscriber);
        }

        //Verify Account
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (PostEntity pe = new PostEntity())
            {
                pe.Configuration.ValidateOnSaveEnabled = false; //This line is added to avoid 
                                                                //confirm password does not match issue
                var v = pe.Subscriber_Table.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();

                if (v != null)
                {
                    v.IsEmailVerified = true;
                    pe.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }
            ViewBag.Status = Status;
            return View();
        }


        //Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        //Login POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(SubscriberLogin login,string ReturnUrl)
        {
            string message = "";
            using (PostEntity pe = new PostEntity())
            {
                var v = pe.Subscriber_Table.Where(a => a.email == login.email).FirstOrDefault();
                if(v != null)
                {
                    if (string.Compare(Crypto.Hash(login.password),v.password)==0)
                    {
                        int timeout = login.RememberMe ? 525600 : 20; //525600 minutes equals 1 year
                        var ticket = new FormsAuthenticationTicket(v.name, login.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);
                        Session["userid"] = v.userid;
                        Session.Timeout = timeout;

                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("ManageUser");
                        }
                    }
                    else
                    {
                        message = "Invalid credentials provided";
                    }
                }
                else
                {
                    message = "Invalid credentials provided";
                }

            }

            ViewBag.Message = message;
            return View(login);
        }

        //Logout
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Post");
        }

        //Manage User action
        [Authorize]
        public ActionResult ManageUser()
        {
            using (PostEntity pe = new PostEntity())
            {
                string userid = Session["userid"].ToString();
                List<Post_Table> article = pe.Post_Table.Where(x => x.userid.Equals(userid) && x.category == true).ToList();
                List<Post_Table> blog = pe.Post_Table.Where(x => x.userid.Equals(userid) && x.category == false).ToList();
                ViewData["Articles"] = article;
                ViewData["Blogs"] = blog;

                return View();
            }
               
        }


        // GET: Subscriber/EditPost/5
        [Authorize]
        public ActionResult EditPost(int id)
        {

            using (PostEntity pe = new PostEntity())
            {
                List<Domain_Table> DomainList = pe.Domain_Table.ToList();
                ViewBag.DomainList = new SelectList(DomainList, "did", "domain");
                var post = pe.Post_Table.Where(x => x.postid == id).FirstOrDefault();
                return View(post);
            }
        }

        // POST: Subscriber/EditPost/5
        [HttpPost]
        [Authorize]
        public ActionResult EditPost(int id, Post_Table post)
        {
            StreamWriter stream = null;
            using (PostEntity pe = new PostEntity())
            {
                List<Domain_Table> DomainList = pe.Domain_Table.ToList();
                ViewBag.DomainList = new SelectList(DomainList, "did", "domain");
            }
            try
            {
                using (PostEntity pe = new PostEntity())
                {
                    //Get all the values of article/blog
                    var postvalues = pe.Post_Table.Where(x => x.postid == id).FirstOrDefault();

                    //set all the values
                    post.postid = postvalues.postid;
                    post.date = postvalues.date;
                    post.category = postvalues.category;
                    post.userid = postvalues.userid;

                    int did = Convert.ToInt32(post.domain);
                    int tid = Convert.ToInt32(post.technology);
                    var d = pe.Domain_Table.Where(x => x.did == did).FirstOrDefault();
                    post.domain = d.domain;
                    var t = pe.Technology_Table.Where(x => x.tid == tid).FirstOrDefault();
                    post.technology = t.technology;
                    post.userid = Session["userid"].ToString();
                    ViewData["Article"] = post;
                    pe.Post_Table.AddOrUpdate(post);
                    pe.SaveChanges();
          
                }
                return View("../Post/ArticleResultView");
            }
            catch(Exception e)
            {
                stream = new StreamWriter(@"D:/EditException.txt");
                stream.WriteLine(e);
                stream.Close();
                return View();
            }
        }

        //GET : Delete Post
        [Authorize]
        public ActionResult DeletePost(int id)
        {
            using (PostEntity pe = new PostEntity())
            {
                return View(pe.Post_Table.Where(x => x.postid == id).FirstOrDefault());
            }
        }

        //POST : Delete Post
        [HttpPost]
        [Authorize]
        public ActionResult DeletePost(int id,FormCollection form)
        {
            try
            {
                using (PostEntity pe = new PostEntity())
                {
                    Post_Table post = pe.Post_Table.Where(x => x.postid == id).FirstOrDefault();
                    pe.Post_Table.Remove(post);
                    pe.SaveChanges();
                }

                return RedirectToAction("ManageUser");
            }
            catch
            {
                return View();
            }
        }



        //Details
        [Authorize]
        public ActionResult Details(int id, Article article)
        {
            using (PostEntity pe = new PostEntity())
            {
                Post_Table post = pe.Post_Table.Where(x => x.postid == id).FirstOrDefault();
                ViewData["Article"] = post;

                if (post.comments != null)
                {
                    article.comments = JsonConvert.DeserializeObject<List<Comment>>(post.comments);
                }
                ViewData["ArticleComments"] = article;
                return View("../Post/ArticleResultView");
            }
             
        }

        [NonAction]
        public bool IsEmailExist(string emailid)
        {
            using (PostEntity pe = new PostEntity())
            {
                var v = pe.Subscriber_Table.Where(a => a.email == emailid).FirstOrDefault();
                return v != null;
            }
        }

        [NonAction]
        public void SendVerificationLinkEmail(string emailid,string activationcode)
        {
            var verifyUrl = "/Subscriber/VerifyAccount/" + activationcode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("vkvishal1508@gmail.com", "Tech Forum");
            var toEmail = new MailAddress(emailid);
            var fromEmailPassword = "Vkvishal@@@1508108254";
            string subject = "Your account is successfully created !";

            string body = "<br/><br/> We are excited to tell you that your Tech Forum account is " 
                + " Successfully created. Please click on the below link to verify your account "
                +" <br/><br/><a href='"+link+"'>"+link+"</a>";
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address,fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true

            })

                try
                {
                    smtp.Send(message);
                }
                catch(Exception e)
                {
                    Response.Redirect("www.google.com");
                }
            
        }

    }
}