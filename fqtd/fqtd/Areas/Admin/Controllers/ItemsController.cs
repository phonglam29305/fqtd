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
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace fqtd.Areas.Admin.Controllers
{
    public class ItemsController : Controller
    {
        private fqtdEntities db = new fqtdEntities();

        //
        // GET: /Admin/Items/

        public ActionResult Index(string keyword = "", int? CategoryID = null, int? BrandID = null, int page = 1)
        {
            var result = from a in db.BrandItems where (a.ItemName.Contains(keyword) || a.ItemName_EN.Contains(keyword)) select a;
            if (CategoryID != null)
                result = result.Where(a => a.tbl_Brands.CategoryID == CategoryID);
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


        public ViewResult ImageList(int id)
        {
            BrandItems item = db.BrandItems.Find(id);
            string path = ConfigurationManager.AppSettings["ItemImageLocaion"] + "\\" + item.ItemID;
            path = Server.MapPath(path);
            List<string> list = new List<string>();
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);

                foreach (string s in files)
                {
                    string filename = Path.GetFileName(s);
                    System.IO.File.Move(s, s.Replace(" ", "_").Replace("-", "_"));
                    if (filename.ToLower().IndexOf(".jpg") >= 0 || filename.ToLower().IndexOf(".png") >= 0 || filename.ToLower().IndexOf(".gif") >= 0)
                        list.Add(ConfigurationManager.AppSettings["ItemImageLocaion"].Replace("~", "../../../..") + "/" + item.ItemID + "/" + filename.Replace(" ", "_").Replace("-", "_"));

                }
            }
            ViewBag.ImageList = list;
            return View(item);
        }
        [HttpPost]
        //[Authorize]
        public ActionResult AddImages(int id, HttpPostedFileBase file)
        {
            var item = db.BrandItems.Find(id);
            if (ModelState.IsValid)
            {
                //HttpPostedFileBase hpf = Request.Files[0] as HttpPostedFileBase;

                if (file != null)
                {
                    char DirSeparator = System.IO.Path.DirectorySeparatorChar;
                    string FilesPath = ConfigurationManager.AppSettings["ItemImageLocaion"];
                    string full_path = Server.MapPath(FilesPath).Replace("Items", "").Replace("AddImages", "").Replace(" ", "_").Replace("-", "_") + "\\" + item.ItemID;
                    FileUpload.UploadFile(file, full_path);
                }
                return RedirectToAction("ImageList", new { id = item.ItemID });
            }
            return View(item);
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

        public ActionResult KeywordBuilder(int itemid = 0)
        {
            var xxx = from i in db.BrandItems
                      join l in db.ItemLocations on i.ItemID equals l.ItemID
                      where l.Street != null && l.Distrist != null && l.City != null && (i.ItemID == itemid || itemid == 0)
                      select new
                      {
                          i.ItemID,
                          i.ItemName,
                          i.ItemName_EN,
                          i.tbl_Brands.BrandName,
                          i.tbl_Brands.BrandName_EN,
                          i.tbl_Brands.tbl_Categories.CategoryName,
                          i.tbl_Brands.tbl_Categories.CategoryName_EN,
                          l.FullAddress,
                          l.Street,
                          l.Distrist,
                          l.City
                      };
            var items = xxx.ToList();
            string keyword = "";
            string keyword_us = "";
            foreach (var item in items)
            {
                keyword = "";
                keyword_us = "";
                var list = db.SP_GetKeyword1(item.Street, item.Distrist, item.City);
                var temp = list.ToList();
                if (temp.Where(a => a.type == 1).ToList().Count == 0)
                {
                    keyword = keyword + ";" + item.BrandName + " " + item.Street;
                    keyword_us = keyword_us + ";" + StripDiacritics(item.BrandName) + " " + StripDiacritics(item.Street);
                }
                if (temp.Where(a => a.type == 2).ToList().Count == 0)
                {
                    keyword = keyword + ";" + item.BrandName + " " + item.Distrist;
                    keyword_us = keyword_us + ";" + StripDiacritics(item.BrandName) + " " + StripDiacritics(item.Distrist);
                }
                if (temp.Where(a => a.type == 3).ToList().Count == 0)
                {
                    keyword = keyword + ";" + item.BrandName + " " + item.City;
                    keyword_us = keyword_us + ";" + StripDiacritics(item.BrandName) + " " + StripDiacritics(item.City);
                }


                list = db.SP_GetKeyword1(item.Street, item.Distrist, item.City);
                foreach (var key in list)
                {
                    if (key.type == 1)//street
                    {
                        string[] words = key.street.Split(';');

                        foreach (var word in words)
                            if (words.Length > 0)
                                keyword = keyword + ";" + item.BrandName + " " + word;
                    }
                    else if (key.type == 2)//district
                    {
                        string[] words = key.district.Split(';');

                        foreach (var word in words)
                            if (words.Length > 0)
                                keyword = keyword + ";" + item.BrandName + " " + word;
                    }
                    if (key.type == 3)//city
                    {
                        string[] words = key.city.Split(';');

                        foreach (var word in words)
                            if (words.Length > 0)
                                keyword = keyword + ";" + item.BrandName + " " + word;
                    }
                    if (key.type == 1)//street
                    {
                        string[] words = key.street_us.Split(';');

                        foreach (var word in words)
                            if (words.Length > 0)
                                keyword_us = keyword_us + ";" + item.BrandName + " " + word;
                    }
                    else if (key.type == 2)//district
                    {
                        string[] words = key.district_us.Split(';');

                        foreach (var word in words)
                            if (words.Length > 0)
                                keyword_us = keyword_us + ";" + item.BrandName + " " + word;
                    }
                    if (key.type == 3)//city
                    {
                        string[] words = key.city_us.Split(';');

                        foreach (var word in words)
                            if (words.Length > 0)
                                keyword_us = keyword_us + ";" + item.BrandName + " " + word;
                    }
                }
                var branditem = db.BrandItems.Find(item.ItemID);
                if (branditem != null)
                {
                    branditem.Keyword = keyword;
                    branditem.Keyword_unsign = keyword_us;
                    db.Entry(branditem).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "BrandName");
            return RedirectToAction("index", "items");
        }

        public static string StripDiacritics(string accented)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");

            string strFormD = accented.Normalize(NormalizationForm.FormD);
            return regex.Replace(strFormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        public ActionResult ItemProperties(int id = 0)
        {
            var result = db.SP_Item_Properties(id);
            TempData["ItemID"] = id;
            return View(result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ItemProperties(string[] MyCheckList)
        {
            int itemid = Convert.ToInt32(TempData["ItemID"]);

            db.SP_RemoveItemProperties(itemid);

            db.SaveChanges();
            foreach (var item in MyCheckList)
            {
                int propertyid = Convert.ToInt32(item);
                var result = db.ItemProperties.Where(a => a.ItemID == itemid && a.PropertyID == propertyid);
                ItemProperties ip = result.FirstOrDefault();
                if (ip != null)
                {
                    ip.PropertyValue = true;
                    db.Entry(ip).State = EntityState.Modified;
                }
                else
                {
                    ip = new ItemProperties();
                    ip.ItemID = itemid;
                    ip.PropertyID = propertyid;
                    ip.PropertyValue = true;
                    db.ItemProperties.Add(ip);
                }
                db.SaveChanges();
                TempData["ItemID"] = null;
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "BrandName");
            return RedirectToAction("index", "items");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}