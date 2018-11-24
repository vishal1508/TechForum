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

        public ActionResult GenerateOverallReport()
        {
            return View();
        }

        public ActionResult GenerateCategoryReport()
        {
            return View();
        }

        public ActionResult DomainSelect(string domlist, TestUser u, String show)
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
                return View("GenerateTestReport", u);
            }
            else
                return View("GenerateTestReport");
        }

        public ActionResult GenerateTestReport(TestUser u, string answer, string inp, string answercat, string Search)
        {
            if (answercat != null)
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
                    u.Domainlist[i] = p;
                    i++;
                }
                ViewBag.domainlistname = u.Domainlist;
                return View(u);
            }

            else if (Search != null)
            {
                PostEntity te = new PostEntity();
                int testid = Int32.Parse(inp);
                var query = (from p in te.Test_Table
                             join q in te.Domain_Table
                             on p.DomainID equals q.did
                             where p.TestId == testid
                             select new
                             {
                                 TestID = p.TestId,
                                 Domain = q.domain,
                                 UserID = p.UserId,
                                 Score = p.Score
                             });
                int i = 0;
                foreach (var p in query)
                {
                    u.Domainlist[i] = p.Domain;
                    u.UserId[i] = p.UserID;
                    u.Score[i] = p.Score;
                    u.TestId[i] = p.TestID;
                    i++;
                }
                u.count = i;

                return View(u);
            }
            else
            {
                return View();
            }
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
                    u.Technology[i] = p.Technology;
                    u.Ratings[i] = p.Rating.Value;
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