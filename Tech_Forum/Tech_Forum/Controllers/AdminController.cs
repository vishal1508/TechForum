using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tech_Forum.Models;

namespace Tech_Forum.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AdLogin(string userid, string password, string Login)
        {
           PostEntity tech = new PostEntity();
            if ( Login!= null)
            {
                var loginquery = (from p in tech.Admin_Table
                                  where p.Username == userid &&
                                  p.Password == password
                                  select new
                                  {
                                      userid = p.Username
                                  }).ToList();
                if (loginquery.Count > 0)
                {
                    return View("GenerateOverallReport");
                }
                else
                {

                    return View("Login", null, model: "Wrong credentials");
                }
            }
            return View();
        }
        public ActionResult GenerateOverallReport()
        {
            return View();
        }

        public ActionResult GenerateCategoryReport()
        {
            return View();
        }

        public ActionResult DomainSelect(string domlist, TestUser u, String show, string showtech, string technolist)
        {
            if (show != null)
            {
                PostEntity tec = new PostEntity();
                var domainlist = (from p in tec.Domain_Table
                                  join q in tec.Test_Table
                                  on p.did equals q.DomainID
                                  where p.domain == domlist
                                  select new
                                  {
                                      domain = p.domain,
                                      UserID = q.UserId,
                                      TestID = q.TestId,
                                      Score = q.Score

                                  }).ToList();
                Session["domainlist"] = domlist;
                u.domainsession = Session["domainlist"].ToString();
                List<QuestionBank> li = new List<QuestionBank>();
                int i = 0;
                foreach (var p in domainlist)
                {

                    u.Domainlist[i] = p.domain;
                    u.UserId[i] = p.UserID;
                    u.TestId[i] = p.TestID;
                    u.Score[i] = p.Score;
                    i++;
                }
                u.count = i;
                var techlist = (from p in tec.Domain_Table
                                join q in tec.Technology_Table
                                on p.did equals q.did
                                where p.domain == domlist
                                select new
                                {
                                    Technology = q.technology
                                }).ToList();
                i = 0;
                foreach (var p in techlist)
                {
                    u.Technologydroplist[i] = p.Technology;
                    i++;
                }
                u.techcount = i;


                i = 0;

                var domainvar = (from p in tec.Domain_Table
                                 select p.domain);
                var count = (from p in tec.Domain_Table
                             select p.domain).Count();
                u.Domaincount = count;
                foreach (var p in domainvar)
                {
                    u.Domaindroplist[i] = p;
                    i++;
                }
                return View("GenerateTestReport", u);


            }
            else if (showtech != null)
            {
                PostEntity tec = new PostEntity();
                u.technologysession = technolist;
                var techlist = (from p in tec.Technology_Table
                                join q in tec.Test_Table
                                on p.tid equals q.TechnologyID
                                where p.technology == u.technologysession
                                select new
                                {
                                    technology = p.technology,
                                    UserID = q.UserId,
                                    TestID = q.TestId,
                                    Score = q.Score

                                }).ToList();

                List<QuestionBank> li = new List<QuestionBank>();
                int i = 0;
                foreach (var p in techlist)
                {

                    u.Technologylist[i] = p.technology;
                    u.UserId[i] = p.UserID;
                    u.TestId[i] = p.TestID;
                    u.Score[i] = p.Score;
                    i++;
                }
                u.count = i;

                i = 0;

                var domainvar = (from p in tec.Domain_Table
                                 select p.domain);
                var count = (from p in tec.Domain_Table
                             select p.domain).Count();
                u.Domaincount = count;
                foreach (var p in domainvar)
                {
                    u.Domaindroplist[i] = p;
                    i++;
                }
                u.domainsession = Session["domainlist"].ToString();
                var techdropdownlist = (from p in tec.Domain_Table
                                        join q in tec.Technology_Table
                                        on p.did equals q.did
                                        where p.domain == u.domainsession
                                        select new
                                        {
                                            Technology = q.technology
                                        }).ToList();
                i = 0;
                foreach (var p in techdropdownlist)
                {
                    u.Technologydroplist[i] = p.Technology;
                    i++;
                }
                u.techcount = i;

                return View("GenerateTestReport", u);

            }
            return View("GenerateTestReport");
        }
        public ActionResult GenerateTestReport(TestUser u, string answer, string inp, string answercat, string Search)
        {


            int i = 0;
            PostEntity tz = new PostEntity();
            var domainvar = (from p in tz.Domain_Table
                             select p.domain);
            var count = (from p in tz.Domain_Table
                         select p.domain).Count();
            u.Domaincount = count;
            foreach (var p in domainvar)
            {
                u.Domaindroplist[i] = p;
                i++;
            }
            ViewBag.domainlistname = u.Domainlist;
            return View(u);


        }
        public ActionResult GenerateUserReport(string answer, string inp, Subscriber_Table si, Post_Table ai, TestUser u)
        {
            if (answer != null)
            {
                PostEntity te = new PostEntity();

                var testsub = (from p in te.Subscriber_Table
                               join t in te.Test_Table
                               on p.userid equals t.UserId
                               where p.userid == inp
                               select new
                               {
                                   ID = p.userid,
                                   Domain = t.DomainID,
                                   Score = t.Score,
                                   Technology = t.TechnologyID,
                                   TesTID = t.TestId,


                               }).ToList();
                int i = 0;
                u.Name = inp;
                foreach (var p in testsub)
                {

                    u.Domain[i] = p.Domain;
                    u.UserId[i] = p.ID;
                    u.Score[i] = p.Score;
                    u.TestId[i] = p.TesTID;
                 
                    if (u.Score[i] <= (0.4) * 10)
                    {
                        u.fail = u.fail + 1;
                    }
                    else
                    {
                        u.pass = u.pass + 1;
                    }
                    i++;

                }
                u.counttest = i;


                var artsubq = (from p in te.Subscriber_Table
                               join e in te.Post_Table
                               on p.userid equals e.userid
                               where p.userid == inp
                               select new
                               {
                                   ID = p.userid,

                                   Title = e.title,
                                   Rating = e.rating,
                                   Technology = e.technology
                               }).ToList();
                i = 0;
                double sum = 0;
                foreach (var p in artsubq)
                {


                    u.Title[i] = p.Title;
                    u.Technologylist[i] = p.Technology;
                    if (p.Rating != null)
                    {
                        u.Ratings[i] = p.Rating.Value;
                    }
                    else
                    {
                        u.Ratings[i] = 0;
                    }
                    sum += u.Ratings[i];
                    i++;
                }
                u.countarticle = i;
                if (u.countarticle > 0)
                    u.averagerating = (sum / u.countarticle);
                else
                    u.averagerating = 0;
                return View(u);


            }
            return View();
        }

    }
}