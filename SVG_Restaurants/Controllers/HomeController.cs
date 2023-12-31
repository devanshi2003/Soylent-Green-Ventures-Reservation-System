﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SVG_Restaurants.Models;
using SVG_Restaurants.ViewModels;
using System.Diagnostics;
using System.Linq;

namespace SVG_Restaurants.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SGVContext _context;   //Test this out

        public HomeController(ILogger<HomeController> logger, SGVContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int customerID)
        {
            var viewModel = new CustomerDetailsVM
            {
                CustomerId = customerID,
                Reservations = _context.Reservations
                    .Include(r => r.Restaurant)
                    .Where(r => r.CustomerId == customerID && r.Completed == false)
                    .OrderBy(r => r.ReservationTiming)
                    .ToList(),
            };

            ViewBag.LoyaltyPoints = _context.Customers
                .Where(c => c.CustomerId == customerID)
                .Select(c => c.LoyaltyPoints ?? 0)
                .FirstOrDefault();

            ViewBag.Status=
                _context.Customers
                .Where(c => c.CustomerId == customerID)
                .Select(c => c.Status ?? "0")
                .FirstOrDefault();

            ViewBag.CustomerName = _context.Customers
                .Where(c => c.CustomerId == customerID)
                .Select(c => c.FirstName)
                .FirstOrDefault();

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}