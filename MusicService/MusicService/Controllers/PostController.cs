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

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Post/Create

        [HttpPost]
        public ActionResult Create(Post post, List<Track> tracks)
        {
            if (ModelState.IsValid)
            {
                db.Posts.Add(post);
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
            var statuses = new List<ViewDataUploadFileResult>();
            statuses = (List<ViewDataUploadFileResult>)TempData["statuses"];
            

            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();

                foreach (var x in statuses)
                {
                    Track track = new Track();
                    TracksInPost tracks = new TracksInPost();
                    track.Name = x.name;
                    track.FileName = x.fullpath;
                    db.Tracks.Add(track);
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


        public ActionResult UploadFile(int? entityId) // optionally receive values specified with Html helper
        {
            // here we can send in some extra info to be included with the delete url 
            var statuses = new List<ViewDataUploadFileResult>();
            for (var i = 0; i < Request.Files.Count; i++)
            {
                var st = FileSaver.StoreFile(x =>
                {
                    x.File = Request.Files[i];
                    //note how we are adding an additional value to be posted with delete request
                    //and giving it the same value posted with upload
                    x.DeleteUrl = Url.Action("DeleteFile", new { entityId = entityId });
                    x.StorageDirectory = Server.MapPath("~/Content/uploads");
                    x.UrlPrefix = "/Content/uploads";
                });
                CreateTrack(st.name, st.url);
                statuses.Add(st);
            }
            //statuses contains all the uploaded files details (if error occurs the check error property is not null or empty)
            //todo: add additional code to generate thumbnail for videos, associate files with entities etc
            //adding thumbnail url for jquery file upload javascript plugin
            //statuses.ForEach(x=>x.thumbnail_url=x.url+"?width=80&height=80"); // uses ImageResizer httpmodule to resize images from this url
            return Json(statuses);
        }

        public void CreateTrack(string name, string fileUrl)
        {
            Track track = new Track(name, fileUrl);
            db.Tracks.Add(track);
            db.SaveChanges();
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