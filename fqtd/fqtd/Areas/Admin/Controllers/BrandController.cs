using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using fqtd.Utils;
using fqtd.Areas.Admin.Models;
using Newtonsoft.Json;
using System.Configuration;

namespace fqtd.Areas.Admin.Controllers
{
    public class BrandController : Controller
    {
        private fqtdEntities db = new fqtdEntities();

        //
        // GET: /Admin/Brands/

        public ActionResult Index()
        {
            var brands = db.Brands.Where(a => a.IsActive).Include(b => b.tbl_Categories);
            return View(brands.ToList());
        }

        
        public ActionResult BrandList(int vn0_en1=0)
        {
            var brands = db.Brands.Where(a => a.IsActive).Include(b => b.tbl_Categories);
            JsonNetResult jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Formatting.Indented;
            jsonNetResult.Data = from a in brands
                                     select new {a.BrandID, a.BrandName, a.Description};
            if (vn0_en1 == 1)
                jsonNetResult.Data = from a in brands
                                     select new { a.BrandID, BrandName=a.BrandName_EN, Description= a.Description_EN };
            return jsonNetResult;
        }
        public ActionResult BrandsByCategory(int id = -1, int vn0_en1 = 0)
        {
            var brands = db.Brands.Where(a => a.IsActive && (id==-1 || a.CategoryID==id)).Include(b => b.tbl_Categories);
            JsonNetResult jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Formatting.Indented;
            jsonNetResult.Data = from a in brands
                                 select new { a.BrandID, a.BrandName, a.Description };
            if (vn0_en1 == 1)
                jsonNetResult.Data = from a in brands
                                     select new { a.BrandID, BrandName = a.BrandName_EN, Description = a.Description_EN };

            return jsonNetResult;
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
            ViewBag.CategoryID = new SelectList(db.Categories.Where(a => a.IsActive), "CategoryID", "CategoryName");
            ViewBag.BrandTypeID = new SelectList(db.BrandTypes.Where(a => a.IsActive), "BrandTypeID", "BrandTypeName");
            return View();
        }

        //
        // POST: /Admin/Brands/Create

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Brands brands, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                brands.IsActive = true;
                brands.CreateDate = DateTime.Now;
                brands.CreateUser = User.Identity.Name;
                string filesPath="", full_path = "";
                if (file != null)
                {
                    char DirSeparator = System.IO.Path.DirectorySeparatorChar;
                    filesPath = ConfigurationManager.AppSettings["BrandMarkerIconLocaion"];
                    full_path = Server.MapPath(filesPath).Replace("Brands","").Replace("Admin","");
                    brands.MarkerIcon = FileUpload.UploadFile(file, full_path);
                }

                db.Brands.Add(brands);
                db.SaveChanges();
                if (file != null)
                {
                    string filename = brands.BrandID + "_" + file.FileName.Replace(" ", "_").Replace("-", "_");
                    brands.MarkerIcon = FileUpload.UploadFile(file, filename, full_path);
                    db.Entry(brands).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories.Where(a => a.IsActive), "CategoryID", "CategoryName", brands.CategoryID);
            ViewBag.BrandTypeID = new SelectList(db.BrandTypes.Where(a => a.IsActive), "BrandTypeID", "BrandTypeName", brands.BrandTypeID);
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
            ViewBag.CategoryID = new SelectList(db.Categories.Where(a => a.IsActive), "CategoryID", "CategoryName", brands.CategoryID);
            ViewBag.BrandTypeID = new SelectList(db.BrandTypes.Where(a => a.IsActive), "BrandTypeID", "BrandTypeName", brands.BrandTypeID);
            return View(brands);
        }

        //
        // POST: /Admin/Brands/Edit/5

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Brands brands)
        {
            if (ModelState.IsValid)
            {
                brands.ModifyDate = DateTime.Now;
                brands.ModifyUser = User.Identity.Name;
                db.Entry(brands).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories.Where(a => a.IsActive), "CategoryID", "CategoryName", brands.CategoryID);
            ViewBag.BrandTypeID = new SelectList(db.BrandTypes.Where(a => a.IsActive), "BrandTypeID", "BrandTypeName", brands.BrandTypeID);
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

            brands.IsActive = true;
            brands.DeleteDate = DateTime.Now;
            brands.DeleteUser = User.Identity.Name;
            db.Entry(brands).State = EntityState.Modified;
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