using SitemapUrl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SitemapUrl.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        Context context = new Context();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult History()
        {
            return PartialView(context.ParentUrls.ToList());
        }
    }
}
