using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MusicService.Models;
using MvcFileUploader;
using MvcFileUploader.Models;

namespace MusicService.Controllers
{
    public class PostController : Controller
    {
        private UsersContext db = new UsersContext();

        //
        // GET: /Post/

        public ActionResult Index()
        {
            return View(db.Posts.ToList());
        }

        //
        // GET: /Post/Details/5

        public ActionResult Details(int id = 0)
        {
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        //
        // GET: /Post/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Post/Create

        [HttpPost]
        public ActionResult Create(Post post)
        {
            var statuses = new List<ViewDataUploadFileResult>();
            statuses = (List<ViewDataUploadFileResult>)TempData["statuses"];

            if (ModelState.IsValid)
            {
                post.User = db.UserProfiles.Single(u => u.UserName == User.Identity.Name);
                db.Posts.Add(post);
                db.SaveChanges();

                foreach (var status in statuses)
                {
                    TracksInPost tracks = new TracksInPost();
                    Track track = new Track(status.name, status.url); 
                    tracks.Post = post;
                    tracks.Track = track;
                    db.TracksInPost.Add(tracks);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(post);
        }


        //
        // GET: /Post/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        //
        // POST: /Post/Edit/5

        [HttpPost]
        public ActionResult Edit(Post post)
        {
            return RedirectToAction("Index");
        }

        //
        // GET: /Post/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        //
        // POST: /Post/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        public Track FindTrackByUrl(string fileUrl)
        {
            return db.Tracks.ToList().Find(x => x.FileName == fileUrl);
        }
        //here i am receving the extra info injected
        public ActionResult DeleteFile(int? entityId, string fileUrl)
        {

            Track track = FindTrackByUrl(fileUrl);
            if (track != null)
            {
                db.Tracks.Remove(track);
                db.SaveChanges();
            }
            var filePath = Server.MapPath("~" + fileUrl);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
            return new HttpStatusCodeResult(200); // trigger success
        }
    }
}