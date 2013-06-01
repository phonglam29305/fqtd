using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using fqtd.Areas.Admin.Models;
using fqtd.Utils;
using Newtonsoft.Json;

namespace fqtd.Areas.Admin.Controllers
{
    public class ItemsController : Controller
    {
        private fqtdEntities db = new fqtdEntities();

        //
        // GET: /Admin/Items/

        public ActionResult Index()
        {
            var branditems = db.BrandItems.Include(b => b.tbl_Brands).Include(b => b.tbl_Item_Location);
            return View(branditems.ToList());
        }

        
        //
        // GET: /Admin/Items/Details/5

        public ActionResult Details(int id = 0)
        {
            BrandItems branditems = db.BrandItems.Find(id);
            if (branditems == null)
            {
                return HttpNotFound();
            }
            return View(branditems);
        }

        //
        // GET: /Admin/Items/Create

        public ActionResult Create()
        {
            ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "BrandName");
            ViewBag.ItemID = new SelectList(db.ItemLocations, "ItemID", "FullAddress");
            return View();
        }

        //
        // POST: /Admin/Items/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BrandItems branditems)
        {
            if (ModelState.IsValid)
            {
                db.BrandItems.Add(branditems);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "BrandName", branditems.BrandID);
            ViewBag.ItemID = new SelectList(db.ItemLocations, "ItemID", "FullAddress", branditems.ItemID);
            return View(branditems);
        }

        //
        // GET: /Admin/Items/Edit/5

        public ActionResult Edit(int id = 0)
        {
            BrandItems branditems = db.BrandItems.Find(id);
            if (branditems == null)
            {
                return HttpNotFound();
            }
            ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "BrandName", branditems.BrandID);
            ViewBag.ItemID = new SelectList(db.ItemLocations, "ItemID", "FullAddress", branditems.ItemID);
            return View(branditems);
        }

        //
        // POST: /Admin/Items/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BrandItems branditems)
        {
            if (ModelState.IsValid)
            {
                db.Entry(branditems).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "BrandName", branditems.BrandID);
            ViewBag.ItemID = new SelectList(db.ItemLocations, "ItemID", "FullAddress", branditems.ItemID);
            return View(branditems);
        }

        //
        // GET: /Admin/Items/Delete/5

        public ActionResult Delete(int id = 0)
        {
            BrandItems branditems = db.BrandItems.Find(id);
            if (branditems == null)
            {
                return HttpNotFound();
            }
            return View(branditems);
        }

        //
        // POST: /Admin/Items/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BrandItems branditems = db.BrandItems.Find(id);
            db.BrandItems.Remove(branditems);
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