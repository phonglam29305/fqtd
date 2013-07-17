﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using fqtd.Areas.Admin.Models;
using fqtd.Utils;
using Newtonsoft.Json;
using System.Configuration;
using System.Web.WebPages;
using System.IO;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Text;

namespace fqtd.Controllers
{
    
    public class ResultController : Controller
    {

        private fqtdEntities db = new fqtdEntities();

        public ActionResult ShowResult(string address, int? range, int? category, int? brand, string search, int? form)
        {
            ViewBag.keywords = ConfigurationManager.AppSettings["metakeywords"];
            ViewBag.description = ConfigurationManager.AppSettings["metakeydescription"];            
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

        public ActionResult ItemByBrandID(int id = -1, string properties = "", int vn0_en1 = 0)
        {
            string path = ConfigurationManager.AppSettings["BrandLogoLocation"].Replace("~", "");
            string c_path = ConfigurationManager.AppSettings["CategoryMarkerIconLocaion"].Replace("~", "");
            string b_path = ConfigurationManager.AppSettings["BrandMarkerIconLocation"].Replace("~", "");
            string i_path = ConfigurationManager.AppSettings["ItemMarkerIconLocaion"].Replace("~", "");

            string[] list = properties.Split(',');
            List<int> items = new List<int>();
            foreach (var x in list)
                if (x != "")
                {
                    try
                    {
                        items.Add(Convert.ToInt32(x));
                    }
                    catch { }

                }

            var itemlist = (from i in db.ItemProperties
                            where items.Contains(i.PropertyID)// >= 0
                            select new { i.ItemID }).Distinct();

            var brands = from i in db.BrandItems
                         join br in db.Brands on i.BrandID equals br.BrandID
                         join c in db.Categories on br.CategoryID equals c.CategoryID
                         where i.BrandID == id
                         select new
                         {
                             i.ItemID,
                             i.ItemName,
                             i.FullAddress,
                             i.Phone,
                             i.Website,
                             i.OpenTime,
                             i.ItemName_EN,
                             i.Description,
                             i.Description_EN,
                             i.Longitude,
                             i.Latitude,
                             Logo = path + "/" + br.Logo,
                             MarkerIcon = i.MarkerIcon == null ? br.MarkerIcon == null ? c_path + "/" + c.MarkerIcon : b_path + "/" + br.MarkerIcon : i_path + "/" + i.MarkerIcon
                         };

            if (itemlist.Count() > 0)
                brands = from i in brands
                         join ip in itemlist on i.ItemID equals ip.ItemID
                         select i;


            JsonNetResult jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Formatting.Indented;
            jsonNetResult.Data = from a in brands
                                 select new { a.ItemID, a.ItemName, a.Description, a.Longitude, a.Latitude, a.FullAddress, a.Website, a.Logo, a.Phone };
            if (vn0_en1 == 1)
                jsonNetResult.Data = from a in brands
                                     select new { a.ItemID, ItemName = a.ItemName_EN, Description = a.Description_EN, a.Longitude, a.Latitude, a.FullAddress, a.Website, a.Logo, a.Phone };

            return jsonNetResult;
        }

        public ActionResult ItemByCategoryID(int id = -1, string properties = "", int vn0_en1 = 0)
        {

            string path = ConfigurationManager.AppSettings["BrandLogoLocation"].Replace("~", "");
            string c_path = ConfigurationManager.AppSettings["CategoryMarkerIconLocaion"].Replace("~", "");
            string b_path = ConfigurationManager.AppSettings["BrandMarkerIconLocation"].Replace("~", "");
            string i_path = ConfigurationManager.AppSettings["ItemMarkerIconLocaion"].Replace("~", "");

            string[] list = properties.Split(',');
            List<int> items = new List<int>();
            foreach (var x in list)
                if (x != "")
                {
                    try
                    {
                        items.Add(Convert.ToInt32(x));
                    }
                    catch { }

                }

            var itemlist = (from i in db.ItemProperties
                            where items.Contains(i.PropertyID)// >= 0
                            select new { i.ItemID }).Distinct();

            var brands = from i in db.BrandItems
                         join br in db.Brands on i.BrandID equals br.BrandID
                         join c in db.Categories on br.CategoryID equals c.CategoryID
                         where c.CategoryID == id
                         select new
                         {
                             i.ItemID,
                             i.ItemName,
                             i.FullAddress,
                             i.Phone,
                             i.Website,
                             i.OpenTime,
                             i.ItemName_EN,
                             i.Description,
                             i.Description_EN,
                             i.Longitude,
                             i.Latitude,
                             Logo = path + "/" + br.Logo,
                             MarkerIcon = i.MarkerIcon == null ? br.MarkerIcon == null ? c_path + "/" + c.MarkerIcon : b_path + "/" + br.MarkerIcon : i_path + "/" + i.MarkerIcon
                         };
            if (itemlist.Count() > 0)
                brands = from i in brands
                         join ip in itemlist on i.ItemID equals ip.ItemID
                         select i;

            JsonNetResult jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Formatting.Indented;
            jsonNetResult.Data = from a in brands
                                 select new { a.ItemID, a.ItemName, a.Description, a.Longitude, a.Latitude, a.FullAddress, a.Website, a.Logo, a.MarkerIcon, a.Phone };
            if (vn0_en1 == 1)
                jsonNetResult.Data = from a in brands
                                     select new { a.ItemID, ItemName = a.ItemName_EN, Description = a.Description_EN, a.Longitude, a.Latitude, a.FullAddress, a.Website, a.Logo, a.MarkerIcon, a.Phone };

            return jsonNetResult;
        }
        public static string StripDiacritics(string accented)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");

            string strFormD = accented.Normalize(NormalizationForm.FormD);
            return regex.Replace(strFormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        public ActionResult ItemByKeyword(string keyword, string properties = "", int vn0_en1 = 0)
        {
            string path = ConfigurationManager.AppSettings["BrandLogoLocation"].Replace("~", "");
            string c_path = ConfigurationManager.AppSettings["CategoryMarkerIconLocaion"].Replace("~", "");
            string b_path = ConfigurationManager.AppSettings["BrandMarkerIconLocation"].Replace("~", "");
            string i_path = ConfigurationManager.AppSettings["ItemMarkerIconLocaion"].Replace("~", "");

            keyword = StripDiacritics(keyword).ToLower();

            string[] list = properties.Split(',');
            List<int> items = new List<int>();
            foreach (var x in list)
                if (x != "")
                {
                    try
                    {
                        items.Add(Convert.ToInt32(x));
                    }
                    catch { }

                }

            var itemlist = (from i in db.ItemProperties
                            where items.Contains(i.PropertyID)// >= 0
                            select new { i.ItemID}).Distinct();

            var brands = from i in db.BrandItems
                         join br in db.Brands on i.BrandID equals br.BrandID
                         join c in db.Categories on br.CategoryID equals c.CategoryID
                         where i.Keyword_unsign.ToLower().Contains(keyword) 
                         || c.Keyword_Unsign.ToLower().Contains(keyword)
                         || br.Keyword_Unsign.ToLower().Contains(keyword)
                         select new
                         {
                             i.ItemID,
                             i.ItemName,
                             i.FullAddress,
                             i.Phone,
                             i.Website,
                             i.OpenTime,
                             i.ItemName_EN,
                             i.Description,
                             i.Description_EN,
                             i.Longitude,
                             i.Latitude,
                             Logo = path + "/" + br.Logo,
                             MarkerIcon = i.MarkerIcon == null ? br.MarkerIcon == null ? c_path + "/" + c.MarkerIcon : b_path + "/" + br.MarkerIcon : i_path + "/" + i.MarkerIcon
                         };
            if (itemlist.Count() > 0)
                brands = from i in brands
                         join ip in itemlist on i.ItemID equals ip.ItemID
                         select i;
                         /*join br in db.Brands on i.BrandID equals br.BrandID
                         join c in db.Categories on br.CategoryID equals c.CategoryID
                         join lo in db.ItemLocations on i.ItemID equals lo.ItemID
                         where i.Keyword_unsign.ToLower().Contains(keyword)
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
                             Logo = path + "/" + br.Logo,
                             MarkerIcon = i.MarkerIcon == null ? br.MarkerIcon == null ? c_path + "/" + c.MarkerIcon : b_path + "/" + br.MarkerIcon : i_path + "/" + i.MarkerIcon
                         };*/
            //string count = "count: " + brands.Count();
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

        public ActionResult Search(int mode = 0, string keyword = "", string currentLocation = "", int categoryid = -1, int brandid = -1, int radious = 1, string properties = "", int vn0_en1 = 0)
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
                return ItemByKeyword(keyword, properties, vn0_en1);
            }
            else // advance search 
            {
                if (brandid != -1)
                {
                    return ItemByBrandID(brandid, properties, vn0_en1);
                }
                else
                {
                    return ItemByCategoryID(categoryid, properties, vn0_en1);
                }
            }
        }

        public ActionResult ItemDetail(int itemID, int vn0_en1 = 0)
        {
            var item = from i in db.BrandItems
                       join br in db.Brands on i.BrandID equals br.BrandID
                       join ca in db.Categories on br.CategoryID equals ca.CategoryID
                       where i.ItemID == itemID
                       select new
                       {
                           i.ItemID
                           ,
                           i.ItemName
                           ,
                           i.ItemName_EN
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
                           i.FullAddress
                           ,
                           i.Longitude
                           ,
                           i.Latitude,
                           ca.MarkerIcon,
                           B_MarkerIcon = br.MarkerIcon,
                           I_MarkerIcon = i.MarkerIcon,
                           br.Logo,
                           br.BrandID,
                           br.CategoryID
                           ,
                           br.BrandName,
                           br.BrandName_EN
                       };
            JsonNetResult jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Formatting.Indented;

            var result = from a in item
                         select new { a.ItemID, a.ItemName, a.BrandName, a.Description, a.Longitude, a.Latitude, a.FullAddress, a.Phone, a.Website, a.OpenTime, a.ClickCount, a.SearchCount };
            if (vn0_en1 == 1)
                result = from a in item
                         select new { a.ItemID, ItemName = a.ItemName_EN, BrandName = a.BrandName_EN, Description = a.Description_EN, a.Longitude, a.Latitude, a.FullAddress, a.Phone, a.Website, a.OpenTime, a.ClickCount, a.SearchCount };
            Dictionary<string, object> list = new Dictionary<string, object>();
            list.Add("ItemDetail", result);
            var temp = item.FirstOrDefault();
            string markerIcon = temp.I_MarkerIcon + "" == "" ? temp.B_MarkerIcon + "" == "" ? ConfigurationManager.AppSettings["CategoryMarkerIconLocation"] + "/" + temp.MarkerIcon : ConfigurationManager.AppSettings["BrandMarkerIconLocaion"] + "/" + temp.B_MarkerIcon : ConfigurationManager.AppSettings["ItemMarkerIconLocaion"] + "/" + temp.I_MarkerIcon;
            list.Add("MakerIcon", markerIcon);
            list.Add("BrandLogo", ConfigurationManager.AppSettings["BrandLogoLocation"] + temp.Logo);
            list.Add("ItemImages", GetImageList(temp.ItemID));
            string path = ConfigurationManager.AppSettings["BrandLogoLocation"].Replace("~", "../..");
            var relateList = from a in db.BrandItems
                             join br in db.Brands on a.BrandID equals br.BrandID
                             join ca in db.Categories on br.CategoryID equals ca.CategoryID
                             where a.ItemID != temp.ItemID && a.BrandID == temp.BrandID
                             select new
                             {
                                 a.ItemID,
                                 ItemName = vn0_en1 == 0 ? a.ItemName : a.ItemName_EN
                                 ,
                                 a.Phone
                                 ,
                                 a.Website
                                 ,
                                 a.FullAddress
                                 ,
                                 Logo = path + "/" + br.Logo
                             };
            list.Add("RelateList", relateList.OrderBy(t => Guid.NewGuid()).Take(5));
            var items = from i in db.BrandItems
                        join br in db.Brands on i.BrandID equals br.BrandID
                        join ca in db.Categories on br.CategoryID equals ca.CategoryID
                        where i.ItemID != itemID && br.CategoryID == temp.CategoryID
                        select new
                        {
                            i.ItemID
                            ,
                            ItemName = vn0_en1 == 0 ? i.ItemName : i.ItemName_EN
                             ,
                            i.Phone
                            ,
                            i.Website
                            ,
                            i.FullAddress
                            ,
                            Logo = path + "/" + br.Logo
                        };
            list.Add("SameCategoryList", items.OrderBy(t => Guid.NewGuid()).Take(5));
            var properties = from a in db.SP_Item_Properties(temp.ItemID)
                             select new { a.PropertyID, a.PropertyValue, PropertyName = vn0_en1 == 0 ? a.PropertyName : a.PropertyName_EN };
            list.Add("PropertyList", properties);
            jsonNetResult.Data = list;
            return jsonNetResult;
        }

        private object GetImageList(int ItemID)
        {
            List<string> images = new List<string>();
            string path = ConfigurationManager.AppSettings["ItemImageLocaion"] + "\\" + ItemID;
            path = Server.MapPath(path);
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);

                foreach (string s in files)
                {
                    string filename = Path.GetFileName(s);
                    System.IO.File.Move(s, s.Replace(" ", "_").Replace("-", "_"));
                    if (filename.ToLower().IndexOf(".jpg") >= 0 || filename.ToLower().IndexOf(".png") >= 0 || filename.ToLower().IndexOf(".gif") >= 0)
                        images.Add(ConfigurationManager.AppSettings["ItemImageLocaion"].Replace("~", "../../../..") + "/" + ItemID + "/" + filename.Replace(" ", "_").Replace("-", "_"));

                }
            }
            return images;
        }

        public ActionResult GetKeyword4Autocomplete(string StringInput)
        {
            var items = db.BrandItems.Where(a => a.Keyword.Length > 0);
            List<string> list = new List<string>();
            foreach (var item in items)
            {
                foreach (var key in item.Keyword.Split(';'))
                {
                    if (key != null && key.ToLower().StartsWith(StringInput.ToLower()))
                        if (!list.Contains(key.ToLower().Trim()))
                            list.Add(key.ToLower().Trim());
                }
            }
            JsonNetResult jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Formatting.Indented;
            jsonNetResult.Data = from a in list select a;
            return jsonNetResult;
        }
    }
}
