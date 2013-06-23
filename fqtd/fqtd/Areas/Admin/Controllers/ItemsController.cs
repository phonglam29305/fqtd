using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using fqtd.Utils;
using fqtd.Areas.Admin.Models;
using Newtonsoft.Json;
using PagedList;
using System.Configuration;
using System.Data;
using System;

namespace fqtd.Areas.Admin.Controllers
{
    public class ItemsController : Controller
    {
        private fqtdEntities db = new fqtdEntities();

        //
        // GET: /Admin/Items/

        public ActionResult Index(string keyword = "", int ?CategoryID=null, int? BrandID = null, int page = 1)
        {
            var result = from a in db.BrandItems where (a.ItemName.Contains(keyword) || a.ItemName_EN.Contains(keyword)) select a;
            if (CategoryID != null)
                result = result.Where(a=>a.tbl_Brands.CategoryID == CategoryID);
            if (BrandID != null)
                result = result.Where(a => a.BrandID == BrandID);
            result = result.OrderBy("ItemName");
            ViewBag.CurrentKeyword = keyword;
            int maxRecords = 20;
            int currentPage = page;
            ViewBag.CurrentPage = page;
            ViewBag.CurrentCategoryID = CategoryID;
            ViewBag.CurrentBrandID = BrandID;
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "BrandName");
            return View(result.ToPagedList(currentPage, maxRecords));
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
        public ActionResult Create(BrandItems branditems, HttpPostedFileBase icon)
        {
            if (ModelState.IsValid)
            {
                branditems.IsActive = true;
                branditems.CreateDate = DateTime.Now;
                branditems.CreateUser = User.Identity.Name;
                string filesPath = "", full_path = "";
                if (icon != null)
                {
                    char DirSeparator = System.IO.Path.DirectorySeparatorChar;
                    filesPath = ConfigurationManager.AppSettings["ItemMarkerIconLocaion"];
                    full_path = Server.MapPath(filesPath).Replace("Brands", "").Replace("Admin", "");
                    branditems.MarkerIcon = FileUpload.UploadFile(icon, full_path);
                }
                db.BrandItems.Add(branditems);
                db.SaveChanges();
                if (icon != null)
                {
                    string filename = branditems.ItemID + "_" + icon.FileName.Replace(" ", "_").Replace("-", "_");
                    branditems.MarkerIcon = FileUpload.UploadFile(icon, filename, full_path);
                    db.Entry(branditems).State = EntityState.Modified;
                    db.SaveChanges();
                }
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
        public ActionResult Edit(BrandItems branditems, HttpPostedFileBase icon)
        {
            if (ModelState.IsValid)
            {
                branditems.ModifyDate = DateTime.Now;
                branditems.ModifyUser = User.Identity.Name;
                string filesPath = "", full_path = "";
                string marker = branditems.MarkerIcon;
                if (icon != null)
                {
                    char DirSeparator = System.IO.Path.DirectorySeparatorChar;
                    filesPath = ConfigurationManager.AppSettings["ItemMarkerIconLocaion"];
                    full_path = Server.MapPath(filesPath).Replace("Brands", "").Replace("Admin", "");
                    branditems.MarkerIcon = FileUpload.UploadFile(icon, full_path);
                }

                db.Entry(branditems).State = EntityState.Modified;
                db.SaveChanges();

                if (marker + "" != "")
                    FileUpload.DeleteFile(marker, full_path);

                if (icon != null)
                {
                    string filename = branditems.ItemID + "_" + icon.FileName.Replace(" ", "_").Replace("-", "_");
                    branditems.MarkerIcon = FileUpload.UploadFile(icon, filename, full_path);
                    db.Entry(branditems).State = EntityState.Modified;
                    db.SaveChanges();
                }
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