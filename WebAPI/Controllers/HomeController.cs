using MvcApplication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            DataContext dbcontext = new DataContext();
            var employees = dbcontext.indicator.ToList();
            return View(employees);
        }

    }
}
