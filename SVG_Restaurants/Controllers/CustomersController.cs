﻿using System;
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

        public async Task<IActionResult> AllCustomers(string nameSearch)
        {
            var users = await _context.Customers.ToListAsync();

            if (!string.IsNullOrEmpty(nameSearch))
            {
                users = users.Where(c =>
                    (
                        c.FirstName != null && c.LastName != null &&
                        (c.FirstName + " " + c.LastName).IndexOf(nameSearch, StringComparison.OrdinalIgnoreCase) >= 0
                    ) ||
                    (
                        c.FirstName != null && c.FirstName.Contains(nameSearch, StringComparison.OrdinalIgnoreCase)
                    ) ||
                    (
                        c.LastName != null && c.LastName.Contains(nameSearch, StringComparison.OrdinalIgnoreCase)
                    )
                ).ToList();
            }

            return View(users);
        }
            // GET: Customers
            public async Task<IActionResult> Index(UserCredentialsVM vm)
        {

            // Check if a user with the provided username and password exists
            var user = await _context.Customers
                .FirstOrDefaultAsync(c => c.Username == vm.username && c.Password == vm.password);
    

            if (user != null)
            {
                if (!string.IsNullOrEmpty(vm.RestaurantId))
                {

                    return RedirectToAction("Create", "Reservations", new { user.CustomerId, vm.RestaurantId });
                }
                return RedirectToAction("Index", "Home", new { user.CustomerId });
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

        public async Task<IActionResult> Redeem(RedeemViewModel vm)
        {

            string cID = Request.Query["CustomerId"];
            int customerId;
            

            if (int.TryParse(cID, out customerId))
            {
                vm.customer = _context.Customers.Where(c => c.CustomerId == customerId).FirstOrDefault();
                return View(vm);

            }
            return View();
        }

        public IActionResult RedeemPoints(RedeemViewModel vm)
        {
            // Assuming you have a Customer model with LoyaltyPoints property
             var customer = _context.Customers.Where(c => c.CustomerId == vm.CustomerId).FirstOrDefault(); 

            if (customer != null)
            {
                if (customer.LoyaltyPoints >= vm.pointsToRedeem)
                {

                    if (vm.pointsToRedeem == 500) {
                        TempData["Points"] = "500";

                    }
                    else
                    {
                        TempData["Points"] = "1500";
                    }
                    customer.LoyaltyPoints -= vm.pointsToRedeem;           

                    TempData["SuccessMessage"] = "Points redeemed successfully.";

                    _context.SaveChanges();

                  
                    return RedirectToAction("Redeem", "Customers", new { vm.CustomerId }); // Redirect to a success page or another appropriate page
                }
                else
                {
                    ModelState.AddModelError("pointsToRedeem", "Insufficient points for redemption.");
                    return View("Redeem", "Customers"); 
                }
            }
            else
            {
                return NotFound();
            }
        }


        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,FirstName,LastName,Email,PhoneNumber,Username,Password,LoyaltyPoints,Status")] Customer customer)
        {
            if (ModelState.IsValid)
            {

                var existingCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Username == customer.Username);
                if (existingCustomer != null)
                {
                    ModelState.AddModelError("Username", "The username is already taken.");
                    return View(customer);
                }

                customer.LoyaltyPoints = 0;
                customer.Status = "None";

                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Customers");
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(CustomerDetailsVM vm)
        {
            if (_context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(vm.CustomerId);
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
        //public async Task<IActionResult> Edit([Bind("CustomerId,FirstName,LastName,Email,PhoneNumber,Username,Password,LoyaltyPoints")] Customer customer)
        public async Task<IActionResult> Edit([Bind("CustomerId,FirstName,LastName,Email,PhoneNumber,Username,Password,LoyaltyPoints,Status")] Customer customer, string? NewPassword)
        {
            ViewBag.submissionSuccess = false;
            bool isPasswordChanged = false;

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCustomer = await _context.Customers.FindAsync(customer.CustomerId);


                    if (existingCustomer.Password != customer.Password)
                    {
                        ModelState.AddModelError("Password", "Incorrect password.");
                        return View(customer);
                    }

                    existingCustomer.FirstName = customer.FirstName;
                    existingCustomer.LastName = customer.LastName;
                    existingCustomer.Email = customer.Email;
                    existingCustomer.PhoneNumber = customer.PhoneNumber;
                    existingCustomer.Username = customer.Username;
                    existingCustomer.Status = existingCustomer.Status;

                    // Update the password if a new one is provided
                    if (!string.IsNullOrEmpty(NewPassword))
                    {
                        existingCustomer.Password = NewPassword;
                        isPasswordChanged = true;
                    }

                    _context.Update(existingCustomer);
                    await _context.SaveChangesAsync();
                    ViewBag.submissionSuccess = true;
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

            }

            if (isPasswordChanged)
            {
                return RedirectToAction("Login");
            }

            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int CustomerId)
        {
            if (CustomerId == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == CustomerId);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int CustomerId)
        {
            try
            {
                // Attempt to find the customer with the specified CustomerId
                var customer = _context.Customers.Single(c => c.CustomerId == CustomerId);

                // Remove the customer entity
                var customerReservation = _context.Reservations.Where(c => c.CustomerId == CustomerId);

                var customerReservations = _context.Reservations.Where(r => r.CustomerId == CustomerId).ToList();

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
