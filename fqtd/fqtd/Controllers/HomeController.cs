using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using fqtd.Areas.Admin.Models;

namespace fqtd.Controllers
{
    public class HomeController : Controller
    {
        private fqtdEntities db = new fqtdEntities();
        public ActionResult Index()
        {
            ViewBag.URL = Request.Url.OriginalString;
            return View("Index");
        }
        public ActionResult Contact()
        {
            tbl_SystemContent tbl_SystemContent = db.tbl_SystemContent.Find(SystemContent.Contact);
            if (tbl_SystemContent == null)
            {
                return HttpNotFound();
            }
            return View(tbl_SystemContent);
        }
        public ActionResult Introduction()
        {
            tbl_SystemContent tbl_SystemContent = db.tbl_SystemContent.Find(SystemContent.Introduction);
            if (tbl_SystemContent == null)
            {
                return HttpNotFound();
            }
            return View(tbl_SystemContent);
        }
        public ActionResult TermAndConditionOfUse()
        {
            tbl_SystemContent tbl_SystemContent = db.tbl_SystemContent.Find(SystemContent.Tern4Use);
            if (tbl_SystemContent == null)
            {
                return HttpNotFound();
            }
            return View(tbl_SystemContent);
        }
        public ActionResult Policy()
        {
            tbl_SystemContent tbl_SystemContent = db.tbl_SystemContent.Find(SystemContent.Policy);
            if (tbl_SystemContent == null)
            {
                return HttpNotFound();
            }
            return View(tbl_SystemContent);
        }
    }
}
