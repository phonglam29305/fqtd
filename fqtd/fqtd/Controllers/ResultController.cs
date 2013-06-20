using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using fqtd.Areas.Admin.Models;
using fqtd.Utils;
using Newtonsoft.Json;

namespace fqtd.Controllers
{
    public class ResultController : Controller
    {

        private fqtdEntities db = new fqtdEntities();

        public ActionResult ShowResult(string address, int? range, int? category, int? brand, string search, int? form)
        {
            ViewBag.address = address;
            ViewBag.range = range;
            ViewBag.category = category;
            ViewBag.brand = brand;
            ViewBag.search = search;
            ViewBag.form = form;
            return View("Index");
        }

        public ActionResult PropertyByCategoryID(int id = -1, int vn0_en1 = 0)
        {
            JsonNetResult jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Formatting.Indented;
            if (vn0_en1 == 0)
                jsonNetResult.Data = from a in db.SP_Category_Properties(id)
                                     select new { a.PropertyID, a.PropertyName, a.Description };
            if (vn0_en1 == 1)
                jsonNetResult.Data = from a in db.SP_Category_Properties(id)
                                     select new { a.PropertyID, PropertyName = a.PropertyName_EN, a.Description };

            return jsonNetResult;
        }

        public ActionResult ItemByBrandID(int id = -1, int vn0_en1 = 0)
        {
            //var brands = db.BrandItems.Where(a => a.IsActive && (id == -1 || a.BrandID == id)).Include(b => b.tbl_Brands);
            var brands = from i in db.BrandItems
                         join br in db.Brands on i.BrandID equals br.BrandID
                         join lo in db.ItemLocations on i.ItemID equals lo.ItemID
                         where i.BrandID == id
                         select new
                         {
                             i.ItemID,
                             i.ItemName,
                             lo.FullAddress,
                             i.Phone,
                             i.Website,
                             i.OpenTime,
                             i.ItemName_EN,
                             i.Description,
                             i.Description_EN,
                             lo.Longitude,
                             lo.Latitude,
                             br.Logo
                         };
            JsonNetResult jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Formatting.Indented;
            jsonNetResult.Data = from a in brands
                                 select new { a.ItemID, a.ItemName, a.Description, a.Longitude, a.Latitude, a.FullAddress, a.Website, a.Logo, a.Phone };
            if (vn0_en1 == 1)
                jsonNetResult.Data = from a in brands
                                     select new { a.ItemID, ItemName = a.ItemName_EN, Description = a.Description_EN, a.Longitude, a.Latitude, a.FullAddress, a.Website, a.Logo, a.Phone };

            return jsonNetResult;
        }

        public ActionResult ItemByCategoryID(int id = -1, int vn0_en1 = 0)
        {
            var brands = from i in db.BrandItems
                         join br in db.Brands on i.BrandID equals br.BrandID
                         join c in db.Categories on br.CategoryID equals c.CategoryID
                         join lo in db.ItemLocations on i.ItemID equals lo.ItemID
                         where c.CategoryID == id
                         select new
                         {
                             i.ItemID,
                             i.ItemName,
                             lo.FullAddress,
                             i.Phone,
                             i.Website,
                             i.OpenTime,
                             i.ItemName_EN,
                             i.Description,
                             i.Description_EN,
                             lo.Longitude,
                             lo.Latitude,
                             br.Logo
                         };
            //db.BrandItems.Where(a => a.IsActive && (id == -1 || a.BrandID == id)).Include(b => b.tbl_Brands);
            JsonNetResult jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Formatting.Indented;
            jsonNetResult.Data = from a in brands
                                 select new { a.ItemID, a.ItemName, a.Description, a.Longitude, a.Latitude, a.FullAddress, a.Website, a.Logo, a.Phone };
            if (vn0_en1 == 1)
                jsonNetResult.Data = from a in brands
                                     select new { a.ItemID, ItemName = a.ItemName_EN, Description = a.Description_EN, a.Longitude, a.Latitude, a.FullAddress, a.Website, a.Logo, a.Phone };

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
                             lo.FullAddress,
                             i.Phone,
                             i.Website,
                             i.OpenTime,
                             i.ItemName_EN,
                             i.Description,
                             i.Description_EN,
                             lo.Longitude,
                             lo.Latitude,
                             br.Logo
                         };
            //db.BrandItems.Where(a => a.IsActive && (id == -1 || a.BrandID == id)).Include(b => b.tbl_Brands);
            JsonNetResult jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Formatting.Indented;
            jsonNetResult.Data = from a in brands
                                 select new { a.ItemID, a.ItemName, a.Description, a.Longitude, a.Latitude, a.FullAddress, a.Website, a.Logo, a.Phone };
            if (vn0_en1 == 1)
                jsonNetResult.Data = from a in brands
                                     select new { a.ItemID, ItemName = a.ItemName_EN, Description = a.Description_EN, a.Longitude, a.Latitude, a.FullAddress, a.Website, a.Logo, a.Phone };

            return jsonNetResult;
        }

        public ActionResult Search(int mode = 0, string keyword = "", string currentLocation = "", int categoryid = -1, int brandid = -1, int radious = 1, int vn0_en1 = 0)
        {
            ViewBag.Mode = mode;
            ViewBag.Keyword = keyword;
            ViewBag.CurrentLocaion = currentLocation;
            ViewBag.CategoryID = categoryid;
            ViewBag.BrandID = brandid;
            ViewBag.Radious = radious;
            ViewBag.CurrentLanguage = vn0_en1;
            if (mode == 0)//search basic
            {
                return ItemByKeyword(keyword, vn0_en1);
            }
            else // advance search 
            {
                if (brandid != -1)
                {
                    return ItemByBrandID(brandid, vn0_en1);
                }
                else
                {
                    return ItemByCategoryID(categoryid, vn0_en1);
                }
            }
        }

        public ActionResult ItemDetail(int itemID, int vn0_en1 = 0)
        {
            var item = from i in db.BrandItems
                       join br in db.Brands on i.BrandID equals br.BrandID
                       join lo in db.ItemLocations on i.ItemID equals lo.ItemID
                       where i.ItemID == itemID
                       select new
                       {
                           i.ItemID
                           ,
                           i.ItemName
                           ,
                           i.ItemName_EN
                           ,
                           i.MarkerIcon
                           ,
                           br.Logo
                           ,
                           i.Phone
                           ,
                           i.Website
                           ,
                           i.OpenTime
                           ,
                           i.ClickCount
                           ,
                           i.SearchCount
                           ,
                           Description = i.Description == "" ? br.Description : i.Description
                           ,
                           Description_EN = i.Description_EN == "" ? br.Description_EN : i.Description_EN
                           ,
                           lo.FullAddress
                           ,
                           lo.Longitude
                           ,
                           lo.Latitude
                       };
            JsonNetResult jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Formatting.Indented;
            jsonNetResult.Data = from a in item
                                 select new { a.ItemID, a.ItemName, a.Description, a.Longitude, a.Latitude, a.FullAddress, a.MarkerIcon, a.Logo, a.Phone, a.Website, a.OpenTime, a.ClickCount, a.SearchCount };
            if (vn0_en1 == 1)
                jsonNetResult.Data = from a in item
                                     select new { a.ItemID, ItemName = a.ItemName_EN, Description = a.Description_EN, a.Longitude, a.Latitude, a.FullAddress, a.MarkerIcon, a.Logo, a.Phone, a.Website, a.OpenTime, a.ClickCount, a.SearchCount };

            return jsonNetResult;
        }
    }
}
