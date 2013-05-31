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

        public ActionResult ItemByBrandID(int id = -1, int vn0_en1 = 0)
        {
            var brands = db.BrandItems.Where(a => a.IsActive && (id == -1 || a.BrandID == id)).Include(b => b.tbl_Brands);
            JsonNetResult jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Formatting.Indented;
            jsonNetResult.Data = from a in brands
                                 select new { a.ItemID, a.ItemName, a.Description };
            if (vn0_en1 == 1)
                jsonNetResult.Data = from a in brands
                                     select new { a.ItemID, ItemName_EN = a.ItemName_EN, Description = a.Description_EN };

            return jsonNetResult;
        }

        public ActionResult ItemByCategoryID(int id = -1, int vn0_en1 = 0)
        {
            var brands = from i in db.BrandItems
                         join br in db.Brands on i.BrandID equals br.BrandID
                         join c in db.Categories on br.CategoryID equals c.CategoryID
                         join lo in db.ItemLocations on i.ItemID  equals lo.ItemID
                         where c.CategoryID == id
                         select new
                         {
                             i.ItemID,
                             i.ItemName,
                             i.ItemName_EN, i.Description, i.Description_EN,
                             lo.Longitude, lo.Latitude
                         };
                //db.BrandItems.Where(a => a.IsActive && (id == -1 || a.BrandID == id)).Include(b => b.tbl_Brands);
            JsonNetResult jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Formatting.Indented;
            jsonNetResult.Data = from a in brands
                                 select new { a.ItemID, a.ItemName, a.Description, a.Longitude, a.Latitude };
            if (vn0_en1 == 1)
                jsonNetResult.Data = from a in brands
                                     select new { a.ItemID, ItemName_EN = a.ItemName_EN, Description = a.Description_EN, a.Longitude, a.Latitude };

            return jsonNetResult;
        }

        public ActionResult ItemByKeyword(string keyword, int vn0_en1 = 0)
        {
            var brands = from i in db.BrandItems
                         join br in db.Brands on i.BrandID equals br.BrandID
                         join c in db.Categories on br.CategoryID equals c.CategoryID
                         join lo in db.ItemLocations on i.ItemID equals lo.ItemID
                         where i.ItemName.Contains(keyword) || i.ItemName_EN.Contains(keyword) || i.Description.Contains(keyword) || i.Description_EN.Contains(keyword)
                         || br.BrandName.Contains(keyword) || br.BrandName_EN.Contains(keyword) || br.Description.Contains(keyword) || br.Description_EN.Contains(keyword)
                         || c.CategoryName.Contains(keyword) || c.CategoryName_EN.Contains(keyword) || c.Description.Contains(keyword) || c.Description_EN.Contains(keyword)
                         select new
                         {
                             i.ItemID,
                             i.ItemName,
                             i.ItemName_EN,
                             i.Description,
                             i.Description_EN,
                             lo.Longitude,
                             lo.Latitude
                         };
            //db.BrandItems.Where(a => a.IsActive && (id == -1 || a.BrandID == id)).Include(b => b.tbl_Brands);
            JsonNetResult jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Formatting.Indented;
            jsonNetResult.Data = from a in brands
                                 select new { a.ItemID, a.ItemName, a.Description, a.Longitude, a.Latitude };
            if (vn0_en1 == 1)
                jsonNetResult.Data = from a in brands
                                     select new { a.ItemID, ItemName_EN = a.ItemName_EN, Description = a.Description_EN, a.Longitude, a.Latitude };

            return jsonNetResult;
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