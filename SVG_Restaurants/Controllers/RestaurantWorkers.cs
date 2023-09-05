using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SVG_Restaurants.Models;

namespace SVG_Restaurants.Controllers
{
    public class RestaurantWorkers : Controller
    {
        private readonly SGVContext _context;

        public RestaurantWorkers(SGVContext context)
        {
            _context = context;
        }

        // GET: RestaurantWorkers
        public async Task<IActionResult> Index()
        {
            var sGVContext = _context.RestaurantWorkers.Include(r => r.Restaurant);
            return View(await sGVContext.ToListAsync());
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
                return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Edit(int id, [Bind("WorkerId,FirstName,LastName,Email,PhoneNumber,Username,Password,RestaurantId")] RestaurantWorker restaurantWorker)
        {
            if (id != restaurantWorker.WorkerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(restaurantWorker);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
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
            return RedirectToAction(nameof(Index));
        }

        private bool RestaurantWorkerExists(int id)
        {
          return (_context.RestaurantWorkers?.Any(e => e.WorkerId == id)).GetValueOrDefault();
        }
    }
}
