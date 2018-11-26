using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
            using (PostEntity pe = new PostEntity())
            {
                List<Domain_Table> DomainList = pe.Domain_Table.ToList();
                ViewBag.DomainList = new SelectList(DomainList, "did", "domain");
            }
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult BrowseArticle(Post_Table post)
        {
            using (PostEntity pe = new PostEntity())
            {
                List<Domain_Table> DomainList = pe.Domain_Table.ToList();
                ViewBag.DomainList = new SelectList(DomainList, "did", "domain");

                int did = Convert.ToInt32(post.domain);
                int tid = Convert.ToInt32(post.technology);
                var d = pe.Domain_Table.Where(x => x.did == did).FirstOrDefault();
                post.domain = d.domain;
                var t = pe.Technology_Table.Where(x => x.tid == tid).FirstOrDefault();
                post.technology = t.technology;


                List<Post_Table> browsearticle = pe.Post_Table.Where(x => x.domain == post.domain && x.technology == post.technology && x.category == true).ToList();
                if (browsearticle.Count > 0)
                {
                    ViewData["browsearticle"] = browsearticle;
                }
                else
                {
                    ViewData["browsearticle"] = null;
                    ViewBag.Message = "No article found";
                }
            }


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

                        ViewData["Article"] = post;
                        pe.Post_Table.Add(post);
                        pe.SaveChanges();
                        
                        return View("ResultView");
                    }  
                }
            }
            catch
            {
                return View();
            }
        }

        public ActionResult PostComment(Post_Table postTable, Comment comment, Article article, Blog blog, string postId, string commentId, string commentContent)
        {
            try
            {
                using (PostEntity pe = new PostEntity())
                {
                    var postIdInt = Convert.ToInt32(postId);

                    var post = pe.Post_Table.Where(x => x.postid == postIdInt).FirstOrDefault();

                    if (post.category)
                    {
                        if (post.comments != null)
                        {
                            article.comments = JsonConvert.DeserializeObject<List<Comment>>(post.comments);
                        }

                        ViewData["Article"] = post;
                        ViewData["ArticleComments"] = article;

                        comment.userid = Session["userid"].ToString();
                        comment.date = DateTime.Now;

                        if (commentContent == null)
                        {
                            if (comment.content_ != null)
                            {
                                if (article.comments.Count() < 10)
                                {
                                    comment.postid = "0" + (article.comments.Count() + 1);
                                }
                                else
                                {
                                    comment.postid = (article.comments.Count() + 1).ToString();
                                }
                                article.comments.Add(comment);
                            }

                            else
                            {
                                ViewData["EmptyComment"] = "Put something atleast!";
                                return View("ResultView");
                            }
                        }

                        else if (commentContent != null)
                        {
                            var foundParentComment = false;
                            while (!foundParentComment)
                            {
                                foundParentComment = AddCommentToParentComment(article.comments, commentId, commentContent);
                            }
                        }

                        var jsonCommentList = JsonConvert.SerializeObject(article.comments);

                        post.comments = jsonCommentList;

                        pe.Post_Table.AddOrUpdate(post);
                        pe.SaveChanges();

                        return View("ResultView");
                    }

                    else
                    {
                        if (post.comments != null)
                        {
                            blog.comments = JsonConvert.DeserializeObject<List<Comment>>(post.comments);
                        }

                        ViewData["Blog"] = post;
                        ViewData["BlogComments"] = blog;

                        comment.userid = Session["userid"].ToString();
                        comment.date = DateTime.Now;

                        if (commentContent == null)
                        {
                            if (blog.comments.Count() < 10)
                            {
                                comment.postid = "0" + (blog.comments.Count() + 1);
                            }
                            else
                            {
                                comment.postid = (blog.comments.Count() + 1).ToString();
                            }
                            blog.comments.Add(comment);
                        }

                        else if (commentContent != null)
                        {
                            var foundParentComment = false;
                            while (!foundParentComment)
                            {
                                foundParentComment = AddCommentToParentComment(blog.comments, commentId, commentContent);
                            }
                        }

                        var jsonCommentList = JsonConvert.SerializeObject(blog.comments);

                        post.comments = jsonCommentList;

                        pe.Post_Table.AddOrUpdate(post);
                        pe.SaveChanges();

                        return View("ResultView");
                    }
                }
            }

            //TODO: Print exception in log
            catch (Exception e)
            {
                return Content(e.ToString());
            }
        }

        //Function to calculate ratings
        public ActionResult CalculateStar(string rate)
        {
            return View("Index");
        }

        //To search for the post/author/tags based on input
        public ActionResult SearchPost(string term)
        {
            using (PostEntity pe = new PostEntity())
            {
                List<Post_Table> searchlist = pe.Post_Table.Where(x => x.tags.Contains(term) || x.title.Contains(term) || x.userid.Contains(term)).ToList();

                if (searchlist.Count > 0)
                {
                    ViewData["searchlist"] = searchlist;
                }
                else
                {
                    ViewData["searchlist"] = null;
                    ViewBag.SearchMessage = "No results found";
                }
            }
            return View("ResultView");
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

        public bool AddCommentToParentComment(List<Comment> comments, string id, string commentContent)
        {
            foreach (var item in comments)
            {
                if (item.postid == id)
                {
                    Comment comment = new Comment();

                    comment.content_ = commentContent;
                    comment.userid = Session["userid"].ToString();
                    comment.date = DateTime.Now;

                    if (item.comments.Count < 10)
                    {
                        comment.postid = id + "0" + (item.comments.Count + 1);
                    }
                    else
                    {
                        comment.postid = id + (item.comments.Count + 1);
                    }

                    item.comments.Add(comment);
                    return true;
                }
                if (AddCommentToParentComment(item.comments, id, commentContent))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
