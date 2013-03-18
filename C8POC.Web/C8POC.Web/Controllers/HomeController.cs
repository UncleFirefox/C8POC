using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace One.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Today()
        {
            return View();
        }

        public ActionResult VirtualMachine()
        {
            return View();
        }

        public ActionResult Play()
        {
            return View();
        }
    }
}
