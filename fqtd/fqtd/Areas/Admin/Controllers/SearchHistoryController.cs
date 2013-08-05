using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fqtd.Areas.Admin.Controllers
{
    public class SearchHistoryController : Controller
    {
        //
        // GET: /Admin/SearchHistory/
        fqtd.Areas.Admin.Models.fqtdEntities db = new Models.fqtdEntities();

        public ActionResult Index()
        {
            return View(db.SearchHistory.OrderByDescending(a=>a.SearchTime).ToList());
        }

    }
}
