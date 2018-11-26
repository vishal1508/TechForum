using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tech_Forum.Models;
using DomainTech;

namespace Tech_Forum.Controllers
{
    public class BlogController : Controller
    {
        // GET: Blog
        public ActionResult Index()
        {
            return View();
        }

        // GET: Blog/Details/5
        public ActionResult Details(int id)
        {
            
            return View();
        }
        [Authorize]
        public ActionResult BrowseBlog()
        {
            
            DomainTech.DomainTechService dt = new DomainTech.DomainTechService();
            List<Domain_Table> DomainList = dt.GetDomainList();
            ViewBag.DomainList = new SelectList(DomainList, "did", "domain");
            return View();
        }

       
        [HttpPost]
        [Authorize]
        public ActionResult BrowseBlog(Post_Table post)
        {
            using (PostEntity pe = new PostEntity())
            {
                DomainTech.DomainTechService dt = new DomainTech.DomainTechService();
                List<Domain_Table> DomainList = dt.GetDomainList();
                ViewBag.DomainList = new SelectList(DomainList, "did", "domain");

                pe.Configuration.ProxyCreationEnabled = false;

                List<Technology_Table> TechnologyList = dt.GetTechforDomain(post.domain);
                ViewBag.TechnologyList = new SelectList(TechnologyList, "tid", "technology");

                int did = Convert.ToInt32(post.domain);
                int tid = Convert.ToInt32(post.technology);

                DomainTechEntity dte = new DomainTechEntity();
                var d = dte.Domain_Table.Where(x => x.did == did).FirstOrDefault();
                post.domain = d.domain;
                var t = dte.Technology_Table.Where(x => x.tid == tid).FirstOrDefault();
                post.technology = t.technology;


                List<Post_Table> browseblog = pe.Post_Table.Where(x => x.domain == post.domain && x.technology == post.technology && x.category == false).ToList();
                if (browseblog.Count > 0)
                {
                    ViewData["browseblog"] = browseblog;
                }
                else
                {
                    ViewData["browseblog"] = null;
                    ViewBag.Message = "No blog found";
                }
            }

            return View();
        }

        // GET: Blog/Create
        [Authorize]
        public ActionResult Create()
        {
            DomainTech.DomainTechService dt = new DomainTech.DomainTechService();
            List<Domain_Table> DomainList = dt.GetDomainList();
            ViewBag.DomainList = new SelectList(DomainList, "did", "domain");
            return View();

        }

        public JsonResult GetTechList(int did)
        {
            using (DomainTechEntity dt = new DomainTechEntity())
            {
                dt.Configuration.ProxyCreationEnabled = false;

                List<Technology_Table> TechList = dt.Technology_Table.Where(x => x.did == did).ToList();
                return Json(TechList, JsonRequestBehavior.AllowGet);
            }

        }

        // POST: Blog/Create
        [HttpPost]
        public ActionResult Create(Post_Table post,Blog blog,Comment comment)
        {
            
            StreamWriter stream = null;
            try
            {
                using (PostEntity pe = new PostEntity())
                {
                    DomainTech.DomainTechService dt = new DomainTech.DomainTechService();
                    List<Domain_Table> DomainList = dt.GetDomainList();
                    ViewBag.DomainList = new SelectList(DomainList, "did", "domain");

                    pe.Configuration.ProxyCreationEnabled = false;

                    List<Technology_Table> TechnologyList = dt.GetTechforDomain(post.domain);
                    ViewBag.TechnologyList = new SelectList(TechnologyList, "tid", "technology");

                    //Get the Domain ID
                    int did = Convert.ToInt32(blog.Post.domain);
                    //Get the Technology ID
                    int tid = Convert.ToInt32(blog.Post.technology);


                    DomainTechEntity dte = new DomainTechEntity();
                    //Convert the Domain ID to Domain Name
                    var d = dte.Domain_Table.Where(x => x.did == did).FirstOrDefault();
                    post.domain = d.domain;

                    //Convert the Technology ID to Technology Name
                    var t = dte.Technology_Table.Where(x => x.tid == tid).FirstOrDefault();
                    post.technology = t.technology;

                    //Check for same title
                    int count = pe.Post_Table.Where(x => x.title == blog.Post.title && x.category == false).Count();

                    if (count > 0)
                    {
                        ViewBag.ErrorMessage = "Please modify the TITLE, Blog found with same TITLE";
                        return View(blog);
                    }
                    else
                    {
                        // Add the date, category and 
                        post.title = blog.Post.title;
                        post.tags = blog.Post.tags;
                        post.content_ = blog.Post.content_;
                        post.date = DateTime.Now;
                        post.category = false;
                        post.userid = Session["userid"].ToString();
                        ViewData["Blog"] = post;
                        pe.Post_Table.Add(post);


                        pe.SaveChanges();
                        return View("../Post/ResultView");
                    }
                    

                }
            }
            catch(DbEntityValidationException dbEx)
            {
                stream = new StreamWriter(@"D:\BlogException.txt");
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        stream.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                stream.Close();
                return View();
            }
        }

        // GET: Blog/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Blog/Edit/5
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

        // GET: Blog/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Blog/Delete/5
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
