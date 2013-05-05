using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using fqtd.Areas.Admin.Models;

namespace fqtd.Areas.Admin.Controllers
{
    public class PropertyController : Controller
    {
        private TimDauEntity db = new TimDauEntity();

        //
        // GET: /Admin/Property/

        public ActionResult Index()
        {
            return View(db.Property.ToList());
        }

        //
        // GET: /Admin/Property/Details/5

        public ActionResult Details(int id = 0)
        {
            PropertyModel propertymodel = db.Property.Find(id);
            if (propertymodel == null)
            {
                return HttpNotFound();
            }
            return View(propertymodel);
        }

        //
        // GET: /Admin/Property/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Admin/Property/Create

        [HttpPost]
        public ActionResult Create(PropertyModel propertymodel)
        {
            if (ModelState.IsValid)
            {
                db.Property.Add(propertymodel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(propertymodel);
        }

        //
        // GET: /Admin/Property/Edit/5

        public ActionResult Edit(int id = 0)
        {
            PropertyModel propertymodel = db.Property.Find(id);
            if (propertymodel == null)
            {
                return HttpNotFound();
            }
            return View(propertymodel);
        }

        //
        // POST: /Admin/Property/Edit/5

        [HttpPost]
        public ActionResult Edit(PropertyModel propertymodel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(propertymodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(propertymodel);
        }

        //
        // GET: /Admin/Property/Delete/5

        public ActionResult Delete(int id = 0)
        {
            PropertyModel propertymodel = db.Property.Find(id);
            if (propertymodel == null)
            {
                return HttpNotFound();
            }
            return View(propertymodel);
        }

        //
        // POST: /Admin/Property/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            PropertyModel propertymodel = db.Property.Find(id);
            db.Property.Remove(propertymodel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}