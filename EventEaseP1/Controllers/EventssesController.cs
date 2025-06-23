using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using EventEaseP1.Models;
using EventEaseP1.Services;



namespace EventEaseP1.Controllers
{
    public class EventssesController : Controller


    {

        private readonly Poepart1Context _context;
        private readonly ILogger<EventssesController> _logger;
        private readonly IAzureStorageService _storageService;


        public EventssesController(Poepart1Context context, ILogger<EventssesController> logger, IAzureStorageService storageService)
        {
            _context = context;
            _logger = logger;
            _storageService = storageService;
        }


        // GET: Eventsses
        public async Task<IActionResult> Index(int? eventTypeId, DateTime? startDate, DateTime? endDate, bool? onlyAvailableVenues)
        {
            var events = _context.Eventsses
                .Include(e => e.EventType)
                .Include(e => e.Venue)
                .AsQueryable();

            if (eventTypeId.HasValue)
                events = events.Where(e => e.EventTypeId == eventTypeId.Value);

            if (startDate.HasValue)
                events = events.Where(e => e.EventDate >= startDate.Value);

            if (endDate.HasValue)
                events = events.Where(e => e.EventDate <= endDate.Value);

            if (onlyAvailableVenues == true)
                events = events.Where(e => e.Venue.IsAvailable);

            ViewBag.EventTypes = new SelectList(_context.EventTypes.ToList(), "EventTypeId", "Name");
            return View(await events.ToListAsync());
        }

        // GET: Eventsses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventss = await _context.Eventsses
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (eventss == null)
            {
                return NotFound();
            }

            return View(eventss);
        }

        // GET: Eventsses/Create
        // GET: Eventsses/Create
        public IActionResult Create()
        {
            try
            {
               // var venues = _context.Venues.ToList();
               // ViewBag.VenueList = venues.Select(v => new SelectListItem
               // {
                  //  Value = v.VenueId.ToString(),
                //    Text = v.Name
             //   });
                ViewBag.VenueList = new SelectList(_context.Venues, "VenueId", "Name");
                ViewBag.EventTypes = new SelectList(_context.EventTypes, "EventTypeId", "Name");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Create GET: {ex.Message}");
                return Problem("Error loading venues");
            }
        }

        // POST: Eventsses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,EventDate,VenueId,EventTypeId,Description,ImageFile")] Eventss eventss)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Upload image if provided
                    if (eventss.ImageFile != null)
                    {
                        eventss.ImageUrl = await _storageService.UploadImageAsync(eventss.ImageFile);
                    }

                    _context.Add(eventss);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating event: {ex.Message}");
                ModelState.AddModelError("", "Error creating event. Please try again.");
            }

            ViewBag.VenueList = new SelectList(_context.Venues, "VenueId", "Name", eventss.VenueId);
            ViewBag.EventTypes = new SelectList(_context.EventTypes, "EventTypeId", "Name", eventss.EventTypeId);
            return View(eventss);
        }

        // GET: Eventsses/TestVenues
        // Add this action to your EventssesController.cs
        public IActionResult TestVenues()
        {
            try
            {
                var venues = _context.Venues.ToList();
                var message = $"Found {venues.Count} venues:\n";
                foreach (var venue in venues)
                {
                    message += $"ID: {venue.VenueId}, Name: {venue.Name}\n";
                }
                return Content(message);
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}\nStack trace: {ex.StackTrace}");
            }
        }
        // GET: Eventsses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventss = await _context.Eventsses
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (eventss == null)
            {
                return NotFound();
            }

            ViewBag.VenueList = new SelectList(_context.Venues, "VenueId", "Name", eventss.VenueId);
            ViewBag.EventTypes = new SelectList(_context.EventTypes, "EventTypeId", "Name", eventss.EventTypeId);
            return View(eventss);
        
        }

        // POST: Eventsses/Edit/5
        // POST: Eventsses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,Name,EventDate,VenueId,EventTypeId,Description,ImageUrl")] Eventss eventss, IFormFile imageFile)
        {
            if (id != eventss.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null)
                    {
                        // Delete old image if it exists
                        if (!string.IsNullOrEmpty(eventss.ImageUrl))
                        {
                            await _storageService.DeleteImageAsync(eventss.ImageUrl);
                        }
                        // Upload new image
                        eventss.ImageUrl = await _storageService.UploadImageAsync(imageFile);
                    }

                    _context.Update(eventss);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventssExists(eventss.EventId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewBag.VenueList = new SelectList(_context.Venues, "VenueId", "Name", eventss.VenueId);
            ViewBag.EventTypes = new SelectList(_context.EventTypes, "EventTypeId", "Name", eventss.EventTypeId);
            return View(eventss);
        }

        // GET: Eventsses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventss = await _context.Eventsses
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (eventss == null)
            {
                return NotFound();
            }

            return View(eventss);
        }

        // POST: Eventsses/Delete/5
        // POST: Eventsses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var eventss = await _context.Eventsses
                    .Include(e => e.Bookings)
                    .FirstOrDefaultAsync(e => e.EventId == id);

                if (eventss == null)
                {
                    return NotFound();
                }

                // Check for active bookings
                if (eventss.Bookings.Any())
                {
                    TempData["Error"] = "Cannot delete event with active bookings.";
                    return RedirectToAction(nameof(Index));
                }

                // Delete image from Azure Storage
                if (!string.IsNullOrEmpty(eventss.ImageUrl))
                {
                    await _storageService.DeleteImageAsync(eventss.ImageUrl);
                }

                _context.Eventsses.Remove(eventss);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Event deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting event: {ex.Message}");
                TempData["Error"] = "Error deleting event. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EventssExists(int id)
        {
            return _context.Eventsses.Any(e => e.EventId == id);
        }
    }
}
