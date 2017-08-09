using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Services;
using core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace web.Controllers
{
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}