﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SVG_Restaurants.Models;

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
            ViewData["AreaId"] = new SelectList(_context.DiningAreas, "AreaId", "AreaId");
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
        public async Task<IActionResult> Create([Bind("ReservationId,CustomerId,RestaurantId,AreaId,ReservationTiming,BanquetId,NumberOfPeople")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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