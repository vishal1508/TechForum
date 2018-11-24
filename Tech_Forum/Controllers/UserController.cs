using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tech_Forum.Models;

namespace Tech_Forum.Controllers
{
    public class UserController : Controller
    {
        // GET: User

        //Index action which returns page "TakeTest"
        [Authorize]
        public ActionResult Index(string sub)
        {
            if (sub != null)
            {
                return View("TakeTest");
            }
            else
            {
                return View();
            }
        }
        //TakeTest action for taking the test
        [ValidateInput(false)]
        [Authorize]
        public ActionResult TakeTest(Test_Table test, string searchtest, string submittechnology, string technology, string submitdomain, QuestionBank qu, string domain, string checkscore, string _1, string _2, string _3, string _4, string _5, string _6, string _7, string _8, string _9, string _10)
        {
            /*provides dropdown to select domain*/
            if (searchtest != null)
            {
                PostEntity tec = new PostEntity();
                var domainlist = (from p in tec.Domain_Table
                                  select new
                                  {
                                      domain = p.domain     //select domain from domain table

                                  }).ToList();

                List<QuestionBank> li = new List<QuestionBank>();
                foreach (var p in domainlist)
                {
                    QuestionBank q = new QuestionBank();
                    q.domainlist = p.domain;
                    li.Add(q);

                }

                return View(li);                        //return the list of domain to the view

            }
            /*Lists the technologies after selecting the respective domain*/
            else if (submitdomain != null)
            {
                PostEntity tec = new PostEntity();



                //fetch technology id
                var techlist = (from p in tec.Technology_Table
                                join q in tec.Domain_Table
                                on p.did equals q.did
                                where q.domain == domain
                                select new
                                {
                                    Technology = p.technology,


                                }).ToList();

                Session["Domain"] = domain;
                var domainlist = (from p in tec.Domain_Table
                                  select new
                                  {
                                      domain = p.domain          //gets the domain list

                                  }).ToList();

                List<QuestionBank> li = new List<QuestionBank>();
                foreach (var p in techlist)
                {
                    QuestionBank q = new QuestionBank();
                    q.techlist = p.Technology;
                    li.Add(q);

                }

                foreach (var p in domainlist)
                {
                    QuestionBank q = new QuestionBank();
                    q.domainlist = p.domain;
                    li.Add(q);

                }


                return View(li);                                  //returns the list of domain and technology

            }
            /*submitting the technology and domain and starting the test*/
            else if (submittechnology != null)
            {

                PostEntity te = new PostEntity();

                //fetch technologyID
                var techidquery = (from p in te.Technology_Table
                                   where p.technology == technology
                                   select new
                                   {
                                       Technology = p.tid
                                   });


                foreach (var p in techidquery)
                {
                    qu.idtech = p.Technology;
                }


                Session["TechnologyID"] = qu.idtech;


                //fetch the questions and options based on TechnologyID
                var query = (from p in te.Question_Bank_Table
                             where p.TechnologyId == qu.idtech
                             select new
                             {
                                 Question = p.Question,
                                 QuestionID = p.QuestionID,
                                 Options = p.Options,
                                 CorrectAnswer = p.CorrectAnswer
                             });
                List<QuestionBank> li = new List<QuestionBank>();
                int no = 1;
                foreach (var p in query)
                {
                    QuestionBank qi = new QuestionBank();
                    qi.QuestionID = p.QuestionID;
                    qi.Question = p.Question;
                    qi.qno = no;
                    qi.CorrectAnswer = p.CorrectAnswer;
                    string[] tempsplit = p.Options.Split(',');
                    qi.Options = tempsplit;
                    li.Add(qi);
                    no++;
                }
                Session["listOfObjects"] = li;
                return View("TestPage", li);
            }


            /*Checking the score after end of test*/
            else if (checkscore != null)
            {

                qu.idtech = Int32.Parse(Session["TechnologyID"].ToString());
                PostEntity te = new PostEntity();


                //Fetch Domain ID
                var iddomain = (from p in te.Technology_Table
                                where p.tid == qu.idtech
                                select new
                                {
                                    DomainID = p.did
                                });

                foreach (var p in iddomain)
                {
                    qu.count = p.DomainID;
                }


                //Based on the technology ID retrieve the correct answers
                var query = (from p in te.Question_Bank_Table
                             where p.TechnologyId == qu.idtech
                             select new
                             {
                                 Question = p.Question,
                                 QuestionID = p.QuestionID,
                                 Options = p.Options,
                                 Answer = p.CorrectAnswer
                             });

                List<string> correctanswer = new List<string>();


                //Append the selected options and enter into test_table
                string selectedoptions = "";

                foreach (var p in query)
                {
                    qu.qid = qu.qid + p.QuestionID + ",";
                    correctanswer.Add(p.Answer);

                }
                selectedoptions = _1 + "," + _2 + "," + _3 + "," + _4 + "," + _5 + "," + _6 + "," + _7 + "," + _8 + "," + _9 + "," + _10;
                Session["selectedoptions"] = selectedoptions;



                /*evaluate the score by checking each of radio button with the correct answer*/
                if (_1 == correctanswer[0])
                {
                    qu.score += 1;
                }
                if (_2 == correctanswer[1])
                {
                    qu.score += 1;
                }
                if (_3 == correctanswer[2])
                {
                    qu.score += 1;
                }
                if (_4 == correctanswer[3])
                {
                    qu.score += 1;
                }
                if (_5 == correctanswer[4])
                {
                    qu.score += 1;
                }
                if (_6 == correctanswer[5])
                {
                    qu.score += 1;
                }
                if (_7 == correctanswer[6])
                {
                    qu.score += 1;
                }
                if (_8 == correctanswer[7])
                {
                    qu.score += 1;
                }
                if (_9 == correctanswer[8])
                {
                    qu.score += 1;
                }
                if (_10 == correctanswer[9])
                {
                    qu.score += 1;
                }

                string userid = Session["userid"].ToString();
                test.UserId = userid;

                test.TechnologyID = (qu.idtech);
                test.DomainID = (qu.count);
                test.SelectedOptions = selectedoptions;
                test.Score = qu.score;
                te.Test_Table.Add(test);

                te.SaveChanges();

                //saving the results of the test in the database
                return View("TestResult", qu);
            }
            else
            {
                return View("TakeTest");
            }
        }

        [Authorize]
        public ActionResult TestResult()
        {
            return View();
        }

        [Authorize]
        public ActionResult TestPage()
        {
            return View();
        }

        [Authorize]
        public ActionResult TestSubmit()
        {
            return View();
        }

        [Authorize]
        public ActionResult ReviewTest(string review)
        {
            if (review != null)
            {
                List<QuestionBank> list = new List<QuestionBank>();
                var varlist = Session["listOfObjects"] as List<QuestionBank>;
                string selected = Session["selectedoptions"].ToString();
                string[] selectedArray = selected.Split(',');
                int i = 0;
                foreach (var p in varlist)
                {
                    QuestionBank obj = new QuestionBank();
                    obj.QuestionID = p.QuestionID;
                    obj.Question = p.Question;
                    obj.Options = p.Options;
                    obj.qno = p.qno;
                    obj.CorrectAnswer = p.CorrectAnswer;
                    obj.selectoptions = selectedArray[i];
                    list.Add(obj);
                    i++;
                }
                return View(list);

            }
            else
            {
                return View();
            }
        }
    }
}