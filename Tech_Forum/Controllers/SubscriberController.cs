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
                    }
                    finally
                    {
                        stream.Close();
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
                            return RedirectToAction("Index", "Post");
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
            var fromEmailPassword = "";
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