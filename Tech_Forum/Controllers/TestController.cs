using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tech_Forum.Models;

namespace Tech_Forum.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Test_List(string test1)
        {
            /*DisplayQuestions dq = new DisplayQuestions();
            if (Convert.ToInt32(Session["question"]) > 4)
            {
                //Session["question"] = 1;
                return View("Test_Result");
            }
            else
            {

            }
            Session["question"] = 1;
            if (test1 != null)
            {

                string ops = null;
                Tech_ForumEntities te = new Tech_ForumEntities();
                int z = Convert.ToInt32(Session["question"]);
                var Question = (from test in te.QuestionBank_Table
                                where test.QuestionID == z
                                select test.Questions).ToList();
                var Options = (from test in te.QuestionBank_Table
                               where test.QuestionID == z
                               select test.Options).ToList();
                foreach (string x in Question)
                {
                    dq.questions = x;
                }

                foreach (string x in Options)
                {
                    ops = x;
                }


                string[] seperator = { "##" };
                string[] op = ops.Split(seperator, StringSplitOptions.None);

                dq.options = op;
                Session["question"] = Convert.ToInt32(Session["question"]) + 1;

                return View("Test1", dq);
            }
            else*/
                return View();



        }
    }
}