﻿using System;
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

        public ActionResult ShowResult(string address, int? range)
        {
            ViewBag.address = address;
            ViewBag.range = range;
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
                         join lo in db.ItemLocations on i.ItemID equals lo.ItemID
                         where c.CategoryID == id
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

        public ActionResult Search(int mode = 0, string keyword = "", string currentLocation = "", int categoryid = -1, int brandid = -1, int radious = 1, int vn0_en1 = 0)
        {
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
    }
}
