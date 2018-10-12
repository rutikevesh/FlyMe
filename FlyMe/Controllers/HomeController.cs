using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FlyMe.Models;
using FlyMe.Data;
using Microsoft.AspNetCore.Authorization;

namespace FlyMe.Controllers
{
    public class HomeController : Controller
    {
        private readonly FlyMeContext _context;

        public HomeController(FlyMeContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            UsersController.CheckIfLoginAndManager(this, _context);
            return View(_context.Airport.ToList());
        }

        public IActionResult About()
        {
            UsersController.CheckIfLoginAndManager(this, _context);
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            UsersController.CheckIfLoginAndManager(this, _context);
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            UsersController.CheckIfLoginAndManager(this, _context);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous]
        public IActionResult AirportLocations()
        {
            var AirportLocations =
                from Airport in _context.Airport
                select new Location
                {
                    longitude = Airport.Longitude,
                    latitude = Airport.Latitude,
                    acronyms = Airport.Acronyms
                };

            return Json(AirportLocations.ToList());
        }

    }

    public class Location
    {
        public double longitude;
        public double latitude;
        public string acronyms;
    }

}
