using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SVG_Restaurants.Models;
using SVG_Restaurants.ViewModels;

namespace SVG_Restaurants.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly SGVContext _context;

        public ReservationsController(SGVContext context)
        {
            _context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {

            return View("Create");

        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Area)
                .Include(r => r.Banquet)
                .Include(r => r.Customer)
                .Include(r => r.Restaurant)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            string rID = Request.Query["restaurantID"];
            int parsedrID;

            var diningAreas = _context.DiningAreas.ToList();


            if (int.TryParse(rID, out parsedrID))
            {
                diningAreas = diningAreas
                    .Where(da => da.RestaurantId == parsedrID)
                    .ToList();
            }

            var banquet = _context.Banquets.Where(c=>c.RestaurantId == parsedrID).ToList();


            // Create a SelectListItem list using AreaName as the display text
            var areaSelectList = diningAreas
                .Select(da => new SelectListItem
                {
                    Value = da.AreaId.ToString(), 
                    Text = da.Area 
                })
                .ToList();

            var banquetList = banquet
               .Select(b => new SelectListItem
               {
                   Value = b.BanquetId.ToString(),
                   Text = b.BanquetName
               })
               .ToList();

            ViewBag.AreaId = new SelectList(areaSelectList, "Value", "Text");
            ViewBag.Banquet = new SelectList(banquetList, "Value", "Text");

            ViewData["BanquetId"] = new SelectList(_context.Banquets, "BanquetId", "BanquetId");
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Email");
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants, "RestaurantId", "RestaurantId");

            return View();
        }




        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservationId,CustomerId,RestaurantId,AreaId,ReservationTiming,BanquetId,NumberOfPeople,HighChairs,SpecialNotes,GuestId")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                var restaurant = await _context.Restaurants.Where(c => c.RestaurantId == reservation.RestaurantId).FirstOrDefaultAsync();

                var dateTimeToCompare = DateTime.ParseExact("2023-09-08 15:30:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                var sumOfNumberOfPeople = _context.Reservations
                    .Where(r => r.ReservationTiming >= dateTimeToCompare)
                    .Sum(r => r.NumberOfPeople);

                //sumOfNumberOfPeople += reservation.NumberOfPeople;

                if (reservation.NumberOfPeople <= restaurant.SeatCapacity)
                {
                    //Decrement the number of available seats
                    restaurant.SeatCapacity -= reservation.NumberOfPeople;

                    // Add the reservation to the database
                    _context.Add(reservation); // Add the reservation entity, not the ViewModel
                    await _context.SaveChangesAsync(); // Save changes to the database

                    if (reservation.CustomerId == null) {
                        return RedirectToAction("Index", "Home");

                    }
                    else
                    {
                        return RedirectToAction("Index", "Home", new { customerID = reservation.CustomerId});
                    }
                }
                    
                else
                {
                    ModelState.AddModelError("NumberOfPeople", "Not enough available seats for this reservation.");

                }


            }
            ViewData["AreaId"] = new SelectList(_context.DiningAreas, "AreaId", "AreaId", reservation.AreaId);
            ViewData["BanquetId"] = new SelectList(_context.Banquets, "BanquetId", "BanquetId", reservation.BanquetId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Email", reservation.CustomerId);
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants, "RestaurantId", "RestaurantId", reservation.RestaurantId);

            return View(reservation);
        }



        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["AreaId"] = new SelectList(_context.DiningAreas, "AreaId", "AreaId", reservation.AreaId);
            ViewData["BanquetId"] = new SelectList(_context.Banquets, "BanquetId", "BanquetId", reservation.BanquetId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Email", reservation.CustomerId);
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants, "RestaurantId", "RestaurantId", reservation.RestaurantId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservationId,CustomerId,RestaurantId,AreaId,ReservationTiming,BanquetId,NumberOfPeople")] Reservation reservation)
        {
            if (id != reservation.ReservationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.ReservationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AreaId"] = new SelectList(_context.DiningAreas, "AreaId", "AreaId", reservation.AreaId);
            ViewData["BanquetId"] = new SelectList(_context.Banquets, "BanquetId", "BanquetId", reservation.BanquetId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Email", reservation.CustomerId);
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants, "RestaurantId", "RestaurantId", reservation.RestaurantId);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Area)
                .Include(r => r.Banquet)
                .Include(r => r.Customer)
                .Include(r => r.Restaurant)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reservations == null)
            {
                return Problem("Entity set 'SGVContext.Reservations'  is null.");
            }
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
          return (_context.Reservations?.Any(e => e.ReservationId == id)).GetValueOrDefault();
        }
    }
}
