using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using fqtd.Areas.Admin.Models;

namespace fqtd.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        private TimDauEntity db = new TimDauEntity();

        //
        // GET: /Category/

        public ActionResult Index()
        {
            return View(db.Category.Where(a=>a.IsActive).ToList());
        }

        //
        // GET: /Category/Details/5

        public ActionResult Details(int id = 0)
        {
            CategoryModel categorymodel = db.Category.Where(a=>a.IsActive && a.CategoryID==id).FirstOrDefault();
            if (categorymodel == null)
            {
                return HttpNotFound();
            }
            return View(categorymodel);
        }

        //
        // GET: /Category/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Category/Create

        [HttpPost]
        public ActionResult Create(CategoryModel categorymodel)
        {
            if (ModelState.IsValid)
            {
                categorymodel.IsActive = true;
                categorymodel.ModifyDate = DateTime.Now;
                categorymodel.ModifyUser = (int)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                db.Category.Add(categorymodel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(categorymodel);
        }

        //
        // GET: /Category/Edit/5

        public ActionResult Edit(int id = 0)
        {
            CategoryModel categorymodel = db.Category.Where(a=>a.IsActive && a.CategoryID == id).FirstOrDefault();
            if (categorymodel == null)
            {
                return HttpNotFound();
            }
            return View(categorymodel);
        }

        //
        // POST: /Category/Edit/5

        [HttpPost]
        public ActionResult Edit(CategoryModel categorymodel)
        {
            if (ModelState.IsValid)
            {
                categorymodel.IsActive = true;
                categorymodel.ModifyDate = DateTime.Now;
                categorymodel.ModifyUser = (int)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                db.Entry(categorymodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(categorymodel);
        }

        //
        // GET: /Category/Delete/5

        public ActionResult Delete(int id = 0)
        {
            CategoryModel categorymodel = db.Category.Where(a => a.IsActive && a.CategoryID == id).FirstOrDefault();
            if (categorymodel == null)
            {
                return HttpNotFound();
            }
            return View(categorymodel);
        }

        //
        // POST: /Category/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CategoryModel categorymodel = db.Category.Where(a => a.IsActive && a.CategoryID == id).FirstOrDefault();
            if (categorymodel == null)
            {
                return HttpNotFound();
            }
            categorymodel.IsActive = false;
            categorymodel.DeleteDate = DateTime.Now;
            categorymodel.DeleteUser = (int)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            db.Entry(categorymodel).State = EntityState.Modified;
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