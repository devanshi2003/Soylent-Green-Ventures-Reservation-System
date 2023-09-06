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
    public class CustomersController : Controller
    {
        private readonly SGVContext _context;

        public CustomersController(SGVContext context)
        {
            _context = context;
        }

        // Customers Login
        public async Task<IActionResult> Login(UserCredentialsVM vm)
        {
            // Retrieve the error message from TempData, if it exists
            string errorMessage = TempData["ErrorMessage"] as string;

            // Create a view model to pass the error message to the view
            vm.errorMessage = errorMessage;

            return View(vm);

            //return View("Login");
        }

        // GET: Customers
        public async Task<IActionResult> Index(UserCredentialsVM vm)
        {

            // Check if a user with the provided username and password exists
            var user = await _context.Customers
                .FirstOrDefaultAsync(c => c.Username == vm.username && c.Password == vm.password);
    

            if (user != null)
            {
                // Redirect to a specific page upon successful login
                return RedirectToAction("Index", "Home", new { CustomerID = user.CustomerId });
            }
            else
            {
                // Handle the case where the credentials do not match
                TempData["ErrorMessage"] = "Invalid username or password.";
                return RedirectToAction("Login");
            }
        }


        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,FirstName,LastName,Email,PhoneNumber,Username,Password,LoyaltyPoints")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Customers");
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,FirstName,LastName,Email,PhoneNumber,Username,Password,LoyaltyPoints")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
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
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int CustomerID)
        {
            try
            {
                // Attempt to find the customer with the specified CustomerId
                var customer = _context.Customers.Single(c => c.CustomerId == CustomerID);

                // Remove the customer entity
                var customerReservation = _context.Reservations.Where(c => c.CustomerId == CustomerID);

                var customerReservations = _context.Reservations.Where(r => r.CustomerId == CustomerID).ToList();

                if (customerReservations.Count > 0)
                {
                    _context.Reservations.RemoveRange(customerReservations);
                    _context.SaveChanges();
                }

                _context.Customers.Remove(customer);

                // Save the changes to the database
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (InvalidOperationException) {
                return NotFound(); // Handle the case where the customer is not found.
            }



        }

        private bool CustomerExists(int id)
        {
          return (_context.Customers?.Any(e => e.CustomerId == id)).GetValueOrDefault();
        }
    }
}
