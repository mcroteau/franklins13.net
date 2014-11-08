using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;
using System.Linq;
using franklins13.net.Models;
using System.Collections.Generic;
using System.IO;
using System.Net;


namespace franklins13.net.Controllers
{

    public class EntryController : Controller
    {

        protected ApplicationDbContext db = new ApplicationDbContext();
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public EntryController()
        {
            this.db = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.db));
        }



        [Authorize]
        public JsonResult Today()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var today = DateTime.Now;

            var query = from e in db.Entries
                            where e.EntryDate == today
                            select e;

            Entry entry = query.FirstOrDefault();

            if (entry == null)
            {
                entry = new Entry();
                entry.EntryDate = today;
                entry.UserID = user.Id;
                db.Entries.Add(entry);
                db.SaveChanges();
            }

            entry.ApplicationUser.Entries = null;


            var settings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var data = JsonConvert.SerializeObject(entry, Formatting.None, settings);

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult SaveEntry()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());

            //http://stackoverflow.com/questions/13041808/mvc-controller-get-json-object-from-http-body
            Stream request = Request.InputStream;
            request.Seek(0, System.IO.SeekOrigin.Begin);
            string json = new StreamReader(request).ReadToEnd();

            Entry entry = null;
            try
            {
                entry = JsonConvert.DeserializeObject<Entry>(json);
            }
            catch (Exception e)
            {
                return Json(e, JsonRequestBehavior.AllowGet);
            }

            var settings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var data = JsonConvert.SerializeObject(entry, Formatting.None, settings);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


    }
}