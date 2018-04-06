using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TudolinkWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string recommendBy)
        {

            if (!string.IsNullOrEmpty(recommendBy))
            {
                Session["recommendBy"] = recommendBy;
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}