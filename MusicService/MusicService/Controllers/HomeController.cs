using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MusicService.Models;
using MvcFileUploader;
using MvcFileUploader.Models;

namespace MusicService.Controllers
{
    public class HomeController : Controller
    {

        private UsersContext db = new UsersContext();

        public ActionResult Index()
        {
            List<Track> trackList = db.Tracks.ToList();
            return View(trackList);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }
        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

       

        public void CreateTrack(string name, string fileUrl)
        {
            Track track = new Track(name,fileUrl);
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
