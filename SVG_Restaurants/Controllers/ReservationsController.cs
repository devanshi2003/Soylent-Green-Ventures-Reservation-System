using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
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
            string rID = Request.Query["RestaurantId"];
            string wID = Request.Query["WorkerId"];
            ViewBag.wID = wID;

            int parsedrID;

            var diningAreas = _context.DiningAreas.ToList();


            if (int.TryParse(rID, out parsedrID))
            {
                diningAreas = diningAreas
                    .Where(da => da.RestaurantId == parsedrID)
                    .ToList();
            }

            var banquet = _context.Banquets.Where(c => c.RestaurantId == parsedrID).ToList();


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
        public async Task<IActionResult> Create([Bind("ReservationId,CustomerId,RestaurantId,AreaId,ReservationTiming,BanquetId,NumberOfPeople,HighChairs,SpecialNotes,GuestId")] Reservation reservation, IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                bool customer_exist = _context.Customers.Any(customer => customer.CustomerId == reservation.CustomerId);
                if (!customer_exist)
                {
                    TempData["CustomerDoesntExist"] = "Customer does not exist !!!";
                    return RedirectToAction("Create", "Reservations", new { RestaurantId = reservation.RestaurantId });
                }

                var timeFrames = new List<(int startHour, int endHour)>
                {
                    (12, 14),
                    (17, 19),
                    (19, 21),
                    (10, 12),
                    (12, 14),
                    (21, 23),
                };


                foreach (var timeFrame in timeFrames)
                {
                    int startHour = timeFrame.startHour;
                    int endHour = timeFrame.endHour;

                    if (reservation.ReservationTiming.Value.Hour >= startHour && reservation.ReservationTiming.Value.Hour < endHour)
                    {

                        var sumOfNumberOfPeople = _context.Reservations
                           .Where(r => r.RestaurantId == reservation.RestaurantId &&
                                    r.ReservationTiming.HasValue &&
                                    r.ReservationTiming.Value.Date == reservation.ReservationTiming.Value.Date &&
                                    r.ReservationTiming.Value.Hour >= startHour &&
                                    r.ReservationTiming.Value.Hour <= endHour)
                           .Sum(r => r.NumberOfPeople);

                        var totalPeople = sumOfNumberOfPeople + reservation.NumberOfPeople;
                        var restaurant = await _context.Restaurants.Where(c => c.RestaurantId == reservation.RestaurantId).FirstOrDefaultAsync();
                        var WorkerId = form["WorkerId"];


                        if (totalPeople <= restaurant.SeatCapacity)
                        {
                            reservation.Completed = false;
                            // Add the reservation to the database
                            _context.Add(reservation); // Add the reservation entity, not the ViewModel
                            await _context.SaveChangesAsync(); // Save changes to the database

                            if (!string.IsNullOrEmpty(WorkerId))
                            {
                                TempData["SuccessMessage"] = "Reservation successfully created!";

                                return RedirectToAction("Home", "RestaurantWorkers", new {RestaurantId = reservation.RestaurantId, WorkerId = WorkerId});
                            }

                            else if (reservation.CustomerId == null)
                            {
                                return RedirectToAction("Create", "Reservations", new { reservation.GuestId, reservation.RestaurantId, reservation.ReservationId });

                            }                     
                            else
                            {
                                return RedirectToAction("Create", "Reservations", new { reservation.CustomerId, reservation.RestaurantId, reservation.ReservationId });
                            }
                            }

                        else
                            {
                                ModelState.AddModelError("NumberOfPeople", "Not enough available seats for this reservation.");

                            }

                        break;

                    }
                }




            }
            ViewData["AreaId"] = new SelectList(_context.DiningAreas, "AreaId", "AreaId", reservation.AreaId);
            ViewData["BanquetId"] = new SelectList(_context.Banquets, "BanquetId", "BanquetId", reservation.BanquetId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Email", reservation.CustomerId);
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants, "RestaurantId", "RestaurantId", reservation.RestaurantId);

            return View(reservation);
        }

        [HttpGet("Reservation/GetAvailableSeats")]
        public JsonResult GetAvailableSeats(DateTime selectedDateTime, int RestaurantId)
        {
            var timeFrames = new List<(int startHour, int endHour)>
                {
                    (12, 14),
                    (17, 19),
                    (19, 21),
                    (10, 12),
                    (12, 14),
                    (21, 23),
                };

            foreach (var timeFrame in timeFrames)
            {
                int startHour = timeFrame.startHour;
                int endHour = timeFrame.endHour;

                if (selectedDateTime.Hour >= startHour && selectedDateTime.Hour < endHour)
                {

                    var sumOfNumberOfPeople = _context.Reservations
                       .Where(r => r.RestaurantId == RestaurantId &&
                                r.ReservationTiming.HasValue &&
                                r.ReservationTiming.Value.Date == selectedDateTime.Date &&
                                r.ReservationTiming.Value.Hour >= startHour &&
                                r.ReservationTiming.Value.Hour <= endHour)
                       .Sum(r => r.NumberOfPeople);


                    Debug.WriteLine(sumOfNumberOfPeople);

                    var seatCapacity = _context.Restaurants
                        .Where(c => c.RestaurantId == RestaurantId)
                        .Select(c => c.SeatCapacity)
                        .FirstOrDefault();


                    int availableSeats = (int)seatCapacity - (int)sumOfNumberOfPeople;

                    return Json(new { availableSeats = availableSeats });
                }

            }
            return Json(new { availableSeats = -1 });

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

            string rID = Request.Query["RestaurantId"];
            int parsedrID;

            var diningAreas = _context.DiningAreas.ToList();

            if (int.TryParse(rID, out parsedrID))
            {
                diningAreas = diningAreas
                    .Where(da => da.RestaurantId == parsedrID)
                    .ToList();
            }

            var banquet = _context.Banquets.Where(c => c.RestaurantId == parsedrID).ToList();

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

            ViewBag.DiningAreas = new SelectList(areaSelectList, "Value", "Text");
            ViewBag.Banquet = new SelectList(banquetList, "Value", "Text");

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
        public async Task<IActionResult> Edit(int id, [Bind("ReservationId,CustomerId,GuestId,RestaurantId,AreaId,ReservationTiming,BanquetId,NumberOfPeople,HighChairs,SpecialNotes, Completed")] Reservation reservation, int? WorkerId)
        {
            ViewBag.updateSuccess = false;
            if (id != reservation.ReservationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    reservation.Completed = false;
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                    ViewBag.updateSuccess = true;
                    if(WorkerId.HasValue)
                    {
                        ViewBag.WorkerID = WorkerId;
                    }
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
                //return RedirectToAction(nameof(Index));
            }
            ViewData["AreaId"] = new SelectList(_context.DiningAreas, "AreaId", "AreaId", reservation.AreaId);
            ViewData["BanquetId"] = new SelectList(_context.Banquets, "BanquetId", "BanquetId", reservation.BanquetId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Email", reservation.CustomerId);
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants, "RestaurantId", "RestaurantId", reservation.RestaurantId);
            return View(reservation);
        }

        public async Task<IActionResult> Complete(int? id)
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
                .Include(r => r.Guest)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }
            return View(reservation);
        }


        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id, int workerID)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }
            ViewBag.WorkerId = workerID;

            var reservation = await _context.Reservations
                .Include(r => r.Area)
                .Include(r => r.Banquet)
                .Include(r => r.Customer)
                .Include(r => r.Guest)
                .Include(r => r.Restaurant)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        [HttpPost, ActionName("Complete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteConfirmed(int id)
        {
            var item = await _context.Reservations
                .FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            if (item.CustomerId != null)
            {
                var customer = await _context.Customers
                    .Where(c => c.CustomerId == item.CustomerId)
                    .FirstOrDefaultAsync();

                if (customer != null)
                {
                    customer.LoyaltyPoints += 50 * item.NumberOfPeople;

                    if (customer.Status != "Gold") {
                        if (customer.LoyaltyPoints >= 500 && customer.LoyaltyPoints < 1500)
                        {
                            customer.Status = "Silver";

                        }
                        else if (customer.LoyaltyPoints >= 1500)
                        {
                            customer.Status = "Gold";
                        }
                    }
                   

                    await _context.SaveChangesAsync();

                    
                }
            }

            var reservation = await _context.Reservations
                .Where(r => r.ReservationId == id)
                .FirstOrDefaultAsync();

            reservation.Completed = true; 

            var WorkerId = HttpContext.Request.Query["WorkerId"];
            var RestaurantId = HttpContext.Request.Query["RestaurantId"];

            //_context.Reservations.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction("Home", "RestaurantWorkers", new { RestaurantId, WorkerId });
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
            int customerId = 0;
            if (reservation.CustomerId != null)
            {
                customerId = (int)reservation.CustomerId;
            }
            var WorkerId = HttpContext.Request.Query["WorkerId"];
            var RestaurantId = HttpContext.Request.Query["RestaurantId"];

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(WorkerId))
            {
                return RedirectToAction("Home", "RestaurantWorkers", new { RestaurantId, WorkerId });
            }

            //return RedirectToAction(nameof(Index));
            return RedirectToAction("Index", "Home", new { customerId = customerId });
            //return Redirect($"/Home/Index/?customerId={customerId}");
        }

        private bool ReservationExists(int id)
        {
            return (_context.Reservations?.Any(e => e.ReservationId == id)).GetValueOrDefault();
        }
    }
}
