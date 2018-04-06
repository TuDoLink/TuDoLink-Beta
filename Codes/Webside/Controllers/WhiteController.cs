using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TudolinkWeb.Controllers
{
    public class WhiteController : Controller
    {
        // GET: White
        public ActionResult Index(string recommendBy)
        {

            if (!string.IsNullOrEmpty(recommendBy))
            {
                Session["recommendBy"] = recommendBy;
            }
            return View();
        }
    }
}