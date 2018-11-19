using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tech_Forum.Models;
namespace Tech_Forum.Controllers
{
    public class PostController : Controller
    {
        // GET: Post
        
        public ActionResult Index()
        {
            return View();
        }

        // GET: Post/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        [Authorize]
        public ActionResult BrowseArticle()
        {
            return View();
        }
        // GET: Post/Create
        [Authorize]
        public ActionResult Create()
        {
            using (PostEntity pe = new PostEntity())
            {
                List<Domain_Table> DomainList = pe.Domain_Table.ToList();
                ViewBag.DomainList = new SelectList(DomainList, "did", "domain");
                return View();
            }  
            
        }

        public JsonResult GetTechList(int did)
        {
            using (PostEntity pe = new PostEntity())
            {
                pe.Configuration.ProxyCreationEnabled = false;
                List<Technology_Table> TechList = pe.Technology_Table.Where(x => x.did == did).ToList();
                return Json(TechList, JsonRequestBehavior.AllowGet);
            }
               
        }

        // POST: Post/Create
        [HttpPost]
        public ActionResult Create(Post_Table post,Article article,Comment comment)
        {
            using (PostEntity pe = new PostEntity())
            {
                List<Domain_Table> DomainList = pe.Domain_Table.ToList();
                ViewBag.DomainList = new SelectList(DomainList, "did", "domain");
            }
            try
            {
                using(PostEntity pe = new PostEntity())
                {
                   
                    int did = Convert.ToInt32(article.Post.domain);
                    int tid = Convert.ToInt32(article.Post.technology);
                    var d = pe.Domain_Table.Where(x => x.did == did).FirstOrDefault();
                    post.domain = d.domain;
                    var t = pe.Technology_Table.Where(x => x.tid == tid).FirstOrDefault();
                    post.technology = t.technology;

                    //Check for same title
                    int count = pe.Post_Table.Where(x => x.title == article.Post.title && x.category == true).Count();

                    if(count > 0)
                    {
                        ViewBag.ErrorMessage = "Please modify the TITLE, Article found with same TITLE";
                        return View(article);
                    }
                    else
                    {
                        post.title = article.Post.title;
                        post.tags = article.Post.tags;
                        post.content_ = article.Post.content_;
                        post.date = DateTime.Now;
                        post.category = true;
                        post.userid = Session["userid"].ToString();

                        pe.Post_Table.Add(post);
                        pe.SaveChanges();
                        return View("UserArticle", post);
                    }
                   
                }

                    
            }
            catch
            {
                return View();
            }
        }

        // GET: Post/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Post/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Post/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Post/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
