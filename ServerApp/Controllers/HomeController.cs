using ServerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ServerApp.Controllers
{
    public class HomeController : Controller
    {
        private DataContext context;

        public HomeController(DataContext context)
        {
            this.context = context;
        }

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
    }
}