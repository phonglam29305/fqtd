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
    public class BrandsController : Controller
    {
        private TimDauEntities db = new TimDauEntities();

        //
        // GET: /Admin/Brands/

        public ActionResult Index()
        {
            var brands = db.Brands.Include(b => b.tbl_Categories);
            return View(brands.ToList());
        }

        //
        // GET: /Admin/Brands/Details/5

        public ActionResult Details(int id = 0)
        {
            Brands brands = db.Brands.Find(id);
            if (brands == null)
            {
                return HttpNotFound();
            }
            return View(brands);
        }

        //
        // GET: /Admin/Brands/Create

        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        //
        // POST: /Admin/Brands/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Brands brands)
        {
            if (ModelState.IsValid)
            {
                db.Brands.Add(brands);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", brands.CategoryID);
            return View(brands);
        }

        //
        // GET: /Admin/Brands/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Brands brands = db.Brands.Find(id);
            if (brands == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", brands.CategoryID);
            return View(brands);
        }

        //
        // POST: /Admin/Brands/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Brands brands)
        {
            if (ModelState.IsValid)
            {
                db.Entry(brands).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", brands.CategoryID);
            return View(brands);
        }

        //
        // GET: /Admin/Brands/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Brands brands = db.Brands.Find(id);
            if (brands == null)
            {
                return HttpNotFound();
            }
            return View(brands);
        }

        //
        // POST: /Admin/Brands/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Brands brands = db.Brands.Find(id);
            db.Brands.Remove(brands);
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