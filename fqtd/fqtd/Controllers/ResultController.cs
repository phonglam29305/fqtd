using System;
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
            string path = ConfigurationManager.AppSettings["BrandLogoLocation"];
            string c_path = ConfigurationManager.AppSettings["CategoryMarkerIconLocaion"];
            string b_path = ConfigurationManager.AppSettings["BrandMarkerIconLocation"];
            string i_path = ConfigurationManager.AppSettings["ItemMarkerIconLocaion"];

            //var brands = db.BrandItems.Where(a => a.IsActive && (id == -1 || a.BrandID == id)).Include(b => b.tbl_Brands);
            var brands = from i in db.BrandItems
                         join br in db.Brands on i.BrandID equals br.BrandID
                         join c in db.Categories on br.CategoryID equals c.CategoryID
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
                             Logo = path + "/" + br.Logo,
                             MarkerIcon = i.MarkerIcon == null ? br.MarkerIcon == null ? c_path + "/" + c.MarkerIcon : b_path + "/" + br.MarkerIcon : i_path + "/" + i.MarkerIcon
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

            string path = ConfigurationManager.AppSettings["BrandLogoLocation"].Replace("~", "..");
            string c_path = ConfigurationManager.AppSettings["CategoryMarkerIconLocaion"].Replace("~", "..");
            string b_path = ConfigurationManager.AppSettings["BrandMarkerIconLocation"].Replace("~", "..");
            string i_path = ConfigurationManager.AppSettings["ItemMarkerIconLocaion"].Replace("~", "..");

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
                             Logo = path + "/" + br.Logo,
                             MarkerIcon = i.MarkerIcon == null ? br.MarkerIcon == null ? c_path + "/" + c.MarkerIcon : b_path + "/" + br.MarkerIcon : i_path + "/" + i.MarkerIcon
                         };
            //db.BrandItems.Where(a => a.IsActive && (id == -1 || a.BrandID == id)).Include(b => b.tbl_Brands);
            JsonNetResult jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Formatting.Indented;
            jsonNetResult.Data = from a in brands
                                 select new { a.ItemID, a.ItemName, a.Description, a.Longitude, a.Latitude, a.FullAddress, a.Website, a.Logo, a.MarkerIcon, a.Phone };
            if (vn0_en1 == 1)
                jsonNetResult.Data = from a in brands
                                     select new { a.ItemID, ItemName = a.ItemName_EN, Description = a.Description_EN, a.Longitude, a.Latitude, a.FullAddress, a.Website, a.Logo, a.MarkerIcon, a.Phone };

            return jsonNetResult;
        }

        public ActionResult ItemByKeyword(string keyword, int vn0_en1 = 0)
        {
            string path = ConfigurationManager.AppSettings["BrandLogoLocation"].Replace("~", "..");
            string c_path = ConfigurationManager.AppSettings["CategoryMarkerIconLocaion"].Replace("~", "..");
            string b_path = ConfigurationManager.AppSettings["BrandMarkerIconLocation"].Replace("~", "..");
            string i_path = ConfigurationManager.AppSettings["ItemMarkerIconLocaion"].Replace("~", "..");

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
                             Logo = path + "/" + br.Logo,
                             MarkerIcon = i.MarkerIcon == null ? br.MarkerIcon == null ? c_path + "/" + c.MarkerIcon : b_path + "/" + br.MarkerIcon : i_path + "/" + i.MarkerIcon
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
                       join ca in db.Categories on br.CategoryID equals ca.CategoryID
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
                           lo.Latitude,
                           ca.MarkerIcon,
                           B_MarkerIcon = br.MarkerIcon,
                           I_MarkerIcon = i.MarkerIcon,
                           br.Logo,
                           br.BrandID,
                           br.CategoryID
                       };
            JsonNetResult jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Formatting.Indented;

            var result = from a in item
                         select new { a.ItemID, a.ItemName, a.Description, a.Longitude, a.Latitude, a.FullAddress, a.Phone, a.Website, a.OpenTime, a.ClickCount, a.SearchCount };
            if (vn0_en1 == 1)
                result = from a in item
                         select new { a.ItemID, ItemName = a.ItemName_EN, Description = a.Description_EN, a.Longitude, a.Latitude, a.FullAddress, a.Phone, a.Website, a.OpenTime, a.ClickCount, a.SearchCount };
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
                             join lo in db.ItemLocations on a.ItemID equals lo.ItemID
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
                                 lo.FullAddress
                                 ,
                                 Logo = path + "/" + br.Logo
                             };
            list.Add("RelateList", relateList);
            var items = from i in db.BrandItems
                        join br in db.Brands on i.BrandID equals br.BrandID
                        join ca in db.Categories on br.CategoryID equals ca.CategoryID
                        join lo in db.ItemLocations on i.ItemID equals lo.ItemID
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
                            lo.FullAddress
                            ,
                            Logo = path + "/" + br.Logo
                        };
            list.Add("SameCategoryList", items);
            var properties = from a in db.SP_Item_Properties(temp.ItemID)
                             select new { a.PropertyID, a.PropertyValue, PropertyName = vn0_en1==0? a.PropertyName :a.PropertyName_EN};
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
    }
}
