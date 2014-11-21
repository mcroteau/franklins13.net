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
using System.Data.Entity;
using franklins13.net.Common;

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


        public ActionResult List()
        {
            return View(db.Entries.ToList());
        }


        [Authorize]
        public ActionResult History()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());

            var EntryList = new List<Entry>();

            var EntryQuery = from e in db.Entries
                             where e.UserID == user.Id
                             select e;

            EntryList.AddRange(EntryQuery.Distinct());

            return View(EntryList);
        }



        [AuthorizeEntryPermission(Permission = ApplicationConstants.EDIT_ENTRY_PERMISSION)]
        public ActionResult Edit(int? id)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var query = from e in db.Entries
                        where (e.Id == id &&
                        e.UserID == user.Id)
                        select e;

            Entry entry = query.FirstOrDefault();

            if (entry == null)
            {
                return RedirectToAction("History");
            }

            return View(entry);
        }





        [Authorize]
        public ActionResult Today()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var today = DateTime.Now;

            var query = from e in db.Entries
                    where (e.EntryDate.Day == today.Day &&
                    e.EntryDate.Month == today.Month &&
                    e.EntryDate.Year == today.Year)
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

            return View(entry);
        }




        [HttpPost]
        [Authorize]
        public JsonResult Save()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());

            //http://stackoverflow.com/questions/13041808/mvc-controller-get-json-object-from-http-body
            Stream request = Request.InputStream;
            request.Seek(0, System.IO.SeekOrigin.Begin);
            string json = new StreamReader(request).ReadToEnd();

            Entry entry = null;
            EntryResponse response;
            try
            {
                entry = JsonConvert.DeserializeObject<Entry>(json);
                var existingEntry = db.Entries.Find(entry.Id);
                if (existingEntry != null)
                {
                    existingEntry.Temperance = entry.Temperance;
                    existingEntry.Silence = entry.Silence;
                    existingEntry.Order = entry.Order;
                    existingEntry.Resolution = entry.Resolution;
                    existingEntry.Frugality = entry.Frugality;
                    existingEntry.Industry = entry.Industry;
                    existingEntry.Sincerity = entry.Sincerity;
                    existingEntry.Justice = entry.Justice;
                    existingEntry.Moderation = entry.Moderation;
                    existingEntry.Cleanliness = entry.Cleanliness;
                    existingEntry.Tranquility = entry.Tranquility;
                    existingEntry.Chastity = entry.Chastity;
                    existingEntry.Humility = entry.Humility;
                    existingEntry.Total = entry.Total;
                    db.SaveChanges();
                    response = GetEntryResponse(existingEntry);
                }
                else
                {
                    response = GetEntryResponse(entry);
                }
            }
            catch (Exception e)
            {
                return Json(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }

            return Json(response, JsonRequestBehavior.AllowGet);

        }




        public JsonResult Data(int? id)
        {
            if (id == null)
            {
                return Json(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }

            Entry entry = db.Entries.Find(id);
            if (entry == null)
            {
                return Json(HttpNotFound());
            }

            EntryResponse response = GetEntryResponse(entry);

            return Json(response, JsonRequestBehavior.AllowGet);
        }




        private EntryResponse GetEntryResponse(Entry entry)
        {
            EntryResponse response = new EntryResponse();
            response.Id = entry.Id;
            response.Total = entry.Total;
            response.UserID = entry.UserID;
            response.EntryDate = entry.EntryDate.ToString("dd MMM yyyy");

            Virtues Virtues = new Virtues();
            Virtues.Temperance = entry.Temperance;
            Virtues.Silence = entry.Silence;
            Virtues.Order = entry.Order;
            Virtues.Resolution = entry.Resolution;
            Virtues.Frugality = entry.Frugality;
            Virtues.Industry = entry.Industry;
            Virtues.Sincerity = entry.Sincerity;
            Virtues.Justice = entry.Justice;
            Virtues.Moderation = entry.Moderation;
            Virtues.Cleanliness = entry.Cleanliness;
            Virtues.Tranquility = entry.Tranquility;
            Virtues.Chastity = entry.Chastity;
            Virtues.Humility = entry.Humility;

            response.Virtues = Virtues;

            return response;
        }



        public class EntryResponse
        {
            public int Id { get; set; }
            public int Total { get; set; }
            public string EntryDate { get; set; }
            public string UserID { get; set; }
            public Virtues Virtues { get; set; }
        }


        public class Virtues
        {
            public int Temperance { get; set; }
            public int Silence { get; set; }
            public int Order { get; set; }
            public int Resolution { get; set; }
            public int Frugality { get; set; }
            public int Industry { get; set; }
            public int Sincerity { get; set; }
            public int Justice { get; set; }
            public int Moderation { get; set; }
            public int Cleanliness { get; set; }
            public int Tranquility { get; set; }
            public int Chastity { get; set; }
            public int Humility { get; set; }
        }


    }
}