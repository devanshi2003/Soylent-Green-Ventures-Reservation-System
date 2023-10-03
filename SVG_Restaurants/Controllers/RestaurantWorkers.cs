using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SVG_Restaurants.Models;
using SVG_Restaurants.ViewModels;

namespace SVG_Restaurants.Controllers
{
    public class RestaurantWorkers : Controller
    {
        private readonly SGVContext _context;

        public RestaurantWorkers(SGVContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Admin()
        {

            return View("Admin");

        }

        public async Task<IActionResult> WorkerDetails()
        {
            var query = await _context.RestaurantWorkers
        .Where(worker => worker.RestaurantId != null) // Replace SomeProperty with the actual property name
        .ToListAsync();

            return View(query);

        }

        //public async Task<IActionResult> Home(int restaurantID)
        public async Task<IActionResult> Home(RestaurantReservationVM vm, int restaurantID)
        {

            var restaurant = await _context.Restaurants
            .Where(c => c.RestaurantId == restaurantID)
            .FirstOrDefaultAsync();

            if (restaurant != null)
            {
                // Restaurant with the specified ID was found, you can use it
                vm.RestaurantId = restaurantID;
                vm.theRestaurant = restaurant;
                vm.reservations = _context.Reservations
                    .Include(c => c.Customer)
                    .Include(g => g.Guest)
                    .Include(r => r.Restaurant)
                    .Include(d => d.Area)
                    .Include(b => b.Banquet)
                    .Where(r => r.RestaurantId == restaurantID && r.Completed == false)
                    .ToList();

                // Return reservations that match search by name
                if(!string.IsNullOrEmpty(vm.nameSearch))
                {
                    var reservationQuery = vm.reservations
                        .Where(c =>
                            (
                                (c.Customer?.FirstName != null && c.Customer?.LastName != null &&
                                (c.Customer.FirstName + " " + c.Customer.LastName)
                                    .IndexOf(vm.nameSearch, StringComparison.OrdinalIgnoreCase) >= 0)
                                ||
                                (c.Guest?.FirstName != null && c.Guest?.LastName != null &&
                                (c.Guest.FirstName + " " + c.Guest.LastName)
                                    .IndexOf(vm.nameSearch, StringComparison.OrdinalIgnoreCase) >= 0)
                            )
                        );

                    vm.reservations = reservationQuery.ToList();
                }
                return View(vm);
            }
            else
            {
                // No matching restaurant was found, handle it accordingly
                return NotFound(); // or any other action/result
            }

        }

        public async Task<IActionResult> CompletedReservations(RestaurantReservationVM vm, int restaurantID)
        {

            var restaurant = await _context.Restaurants
            .Where(c => c.RestaurantId == restaurantID)
            .FirstOrDefaultAsync();

            if (restaurant != null)
            {
                // Restaurant with the specified ID was found, you can use it
                vm.RestaurantId = restaurantID;
                vm.theRestaurant = restaurant;
                vm.reservations = _context.Reservations
                    .Include(c => c.Customer)
                    .Include(g => g.Guest)
                    .Include(r => r.Restaurant)
                    .Include(d => d.Area)
                    .Include(b => b.Banquet)
                    .Where(r => r.RestaurantId == restaurantID && r.Completed == true)
                    .ToList();

                // Return reservations that match search by name
                if (!string.IsNullOrEmpty(vm.nameSearch))
                {
                    var reservationQuery = vm.reservations
                        .Where(c =>
                            (
                                (c.Customer?.FirstName != null && c.Customer?.LastName != null &&
                                (c.Customer.FirstName + " " + c.Customer.LastName)
                                    .IndexOf(vm.nameSearch, StringComparison.OrdinalIgnoreCase) >= 0)
                                ||
                                (c.Guest?.FirstName != null && c.Guest?.LastName != null &&
                                (c.Guest.FirstName + " " + c.Guest.LastName)
                                    .IndexOf(vm.nameSearch, StringComparison.OrdinalIgnoreCase) >= 0)
                            )
                        );

                    vm.reservations = reservationQuery.Where(c=> c.Completed == true).ToList();
                }
                return View(vm);
            }
            else
            {
                // No matching restaurant was found, handle it accordingly
                return NotFound(); // or any other action/result
            }

        }

        // Restaurant staff Login
        public async Task<IActionResult> Login(UserCredentialsVM vm)
        {
            // Retrieve the error message from TempData, if it exists
            string errorMessage = TempData["ErrorMessage"] as string;

            // Create a view model to pass the error message to the view
            vm.errorMessage = errorMessage;

            return View(vm);
        }

        // GET: RestaurantWorkers
        public async Task<IActionResult> Index(UserCredentialsVM vm)
        {

            // Check if a user with the provided username and password exists
            var user = await _context.RestaurantWorkers
                .FirstOrDefaultAsync(c => c.Username == vm.username && c.Password == vm.password);


            var admin = await _context.RestaurantWorkers
                         .FirstOrDefaultAsync(c => c.Username == "admin1");

            

            if (user != null)
            {
                ViewBag.WorkerId = user.WorkerId;

                if (vm.username == admin.Username)
                {
                    if (admin.Password != vm.password && vm.username != admin.Username)
                    {
                        // Handle the case where the credentials do not match
                        TempData["ErrorMessage"] = "Invalid username or password.";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        return RedirectToAction("Admin", "RestaurantWorkers");
                    }
                }
                else {
                    return RedirectToAction("Home", "RestaurantWorkers", new { user.RestaurantId, user.WorkerId });

                }
            }
            else
            {
                // Handle the case where the credentials do not match
                TempData["ErrorMessage"] = "Invalid username or password.";
                return RedirectToAction("Login");
            }
        }

        // GET: RestaurantWorkers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.RestaurantWorkers == null)
            {
                return NotFound();
            }

            var restaurantWorker = await _context.RestaurantWorkers
                .Include(r => r.Restaurant)
                .FirstOrDefaultAsync(m => m.WorkerId == id);
            if (restaurantWorker == null)
            {
                return NotFound();
            }

            return View(restaurantWorker);
        }

        // GET: RestaurantWorkers/Create
        public IActionResult Create()
        {
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants, "RestaurantId", "RestaurantId");
            return View();
        }

        // POST: RestaurantWorkers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkerId,FirstName,LastName,Email,PhoneNumber,Username,Password,RestaurantId")] RestaurantWorker restaurantWorker)
        {
            if (ModelState.IsValid)
            {
                _context.Add(restaurantWorker);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Created";

                return RedirectToAction("WorkerDetails", "RestaurantWorkers");
            }
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants, "RestaurantId", "RestaurantId", restaurantWorker.RestaurantId);
            return View(restaurantWorker);
        }

        // GET: RestaurantWorkers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.RestaurantWorkers == null)
            {
                return NotFound();
            }

            var restaurantWorker = await _context.RestaurantWorkers.FindAsync(id);
            if (restaurantWorker == null)
            {
                return NotFound();
            }
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants, "RestaurantId", "RestaurantId", restaurantWorker.RestaurantId);
            return View(restaurantWorker);
        }

        // POST: RestaurantWorkers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("WorkerId,FirstName,LastName,Email,PhoneNumber,Username,Password,RestaurantId")] RestaurantWorker restaurantWorker)
        {
           

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(restaurantWorker);
                    await _context.SaveChangesAsync();
                    TempData["EditDone"] = "Edit";

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestaurantWorkerExists(restaurantWorker.WorkerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("WorkerDetails", "RestaurantWorkers");
            }
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants, "RestaurantId", "RestaurantId", restaurantWorker.RestaurantId);
            return View(restaurantWorker);
        }

        // GET: RestaurantWorkers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.RestaurantWorkers == null)
            {
                return NotFound();
            }

            var restaurantWorker = await _context.RestaurantWorkers
                .Include(r => r.Restaurant)
                .FirstOrDefaultAsync(m => m.WorkerId == id);
            if (restaurantWorker == null)
            {
                return NotFound();
            }

            return View(restaurantWorker);
        }

        // POST: RestaurantWorkers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.RestaurantWorkers == null)
            {
                return Problem("Entity set 'SGVContext.RestaurantWorkers'  is null.");
            }
            var restaurantWorker = await _context.RestaurantWorkers.FindAsync(id);
            if (restaurantWorker != null)
            {
                _context.RestaurantWorkers.Remove(restaurantWorker);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("WorkerDetails", "RestaurantWorkers");

        }

        private bool RestaurantWorkerExists(int id)
        {
          return (_context.RestaurantWorkers?.Any(e => e.WorkerId == id)).GetValueOrDefault();
        }
    }
}
