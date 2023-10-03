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
    public class RestaurantsController : Controller
    {
        private readonly SGVContext _context;

        public RestaurantsController(SGVContext context)
        {
            _context = context;
        }

        // GET: BambooLeaf
        public async Task<IActionResult> BambooLeaf(int? CustomerID, int RestaurantId, RestaurantVM vm)
        {
            if(CustomerID.HasValue)
            {
                vm.CustomerId = CustomerID;
            }
            vm.RestaurantId = RestaurantId;

            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync();
            return View(vm);
        }
        // GET: LaOeste
        public async Task<IActionResult> LaOeste(int? CustomerID, int RestaurantId, RestaurantVM vm)
        {
            if (CustomerID.HasValue)
            {
                vm.CustomerId = CustomerID;
            }
            vm.RestaurantId = RestaurantId;

            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync();
            return View(vm);
        }

        // GET: Mexikana
        public async Task<IActionResult> Mexikana(int? CustomerID, int RestaurantId, RestaurantVM vm)
        {
            if (CustomerID.HasValue)
            {
                vm.CustomerId = CustomerID;
            }
            vm.RestaurantId = RestaurantId;

            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync();
            
            ViewBag.AvailableSeats = 50;

            return View(vm);
        }

        // GET: Restaurants
        public async Task<IActionResult> Index()
        {
              return _context.Restaurants != null ? 
                          View(await _context.Restaurants.ToListAsync()) :
                          Problem("Entity set 'SGVContext.Restaurants'  is null.");
        }

        // GET: Restaurants/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Restaurants == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(m => m.RestaurantId == id);
            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }

        // GET: Restaurants/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Restaurants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RestaurantId,RestaurantAddress,RestaurantName")] Restaurant restaurant)
        {
            if (ModelState.IsValid)
            {
                _context.Add(restaurant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(restaurant);
        }

        // GET: Restaurants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Restaurants == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }
            return View(restaurant);
        }

        // POST: Restaurants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RestaurantId,RestaurantAddress,RestaurantName")] Restaurant restaurant)
        {
            if (id != restaurant.RestaurantId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(restaurant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestaurantExists(restaurant.RestaurantId))
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
            return View(restaurant);
        }

        // GET: Restaurants/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Restaurants == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(m => m.RestaurantId == id);
            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }

        // POST: Restaurants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Restaurants == null)
            {
                return Problem("Entity set 'SGVContext.Restaurants'  is null.");
            }
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant != null)
            {
                _context.Restaurants.Remove(restaurant);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RestaurantExists(int id)
        {
          return (_context.Restaurants?.Any(e => e.RestaurantId == id)).GetValueOrDefault();
        }
    }
}
