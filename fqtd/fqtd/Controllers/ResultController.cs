using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fqtd.Controllers
{
    public class ResultController : Controller
    {       

        public ActionResult ShowResult(string address, int? range)
        {
            ViewBag.address = address;
            ViewBag.range = range;
            return View("Index");
        }

    }
}
