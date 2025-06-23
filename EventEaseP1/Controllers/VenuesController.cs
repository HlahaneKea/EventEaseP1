using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEaseP1.Models;
using EventEaseP1.Services;

namespace EventEaseP1.Controllers
{
    public class VenuesController : Controller
    {
        private readonly Poepart1Context _context;
        private readonly IAzureStorageService _storageService;
        private readonly ILogger<VenuesController> _logger;

        public VenuesController(Poepart1Context context,
                              IAzureStorageService storageService,
                              ILogger<VenuesController> logger)
        {
            _context = context;
            _storageService = storageService;
            _logger = logger;
        }


        // GET: Venues
        public async Task<IActionResult> Index()
        {
            return View(await _context.Venues.ToListAsync());
        }

        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Venues/Create
        // POST: Venues/Create
        // POST: Venues/Create
        // POST: Venues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Location,Capacity,ImageFile,IsAvailable")] Venue venue)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Upload image if provided
                    if (venue.ImageFile != null)
                    {
                        venue.ImageUrl = await _storageService.UploadImageAsync(venue.ImageFile);
                    }

                    // Set VenueId (using your existing logic)
                    var maxId = await _context.Venues.MaxAsync(v => (int?)v.VenueId) ?? 0;
                    venue.VenueId = maxId + 1;

                    _context.Add(venue);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating venue: {ex.Message}");
                ModelState.AddModelError("", "Error creating venue. Please try again.");
            }
            return View(venue);
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues.FindAsync(id);
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        // POST: Venues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueId,Name,Location,Capacity,ImageUrl,IsAvailable")] Venue venue)
        {
            if (id != venue.VenueId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueId))
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
            return View(venue);
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var venue = await _context.Venues
                    .Include(v => v.Bookings)
                    .Include(v => v.Eventsses) // Also check for associated events
                    .FirstOrDefaultAsync(v => v.VenueId == id);

                if (venue == null)
                {
                    return NotFound();
                }

                // Check for active bookings
                if (venue.Bookings.Any())
                {
                    TempData["Error"] = "Cannot delete venue with active bookings.";
                    return RedirectToAction(nameof(Index));
                }

                // Check for associated events
                if (venue.Eventsses.Any())
                {
                    TempData["Error"] = "Cannot delete venue with associated events.";
                    return RedirectToAction(nameof(Index));
                }

                // Delete image from Azure Storage
                if (!string.IsNullOrEmpty(venue.ImageUrl))
                {
                    await _storageService.DeleteImageAsync(venue.ImageUrl);
                }

                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Venue deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting venue: {ex.Message}");
                TempData["Error"] = "Error deleting venue. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }


        private bool VenueExists(int id)
        {
            return _context.Venues.Any(e => e.VenueId == id);
        }
    }
}
