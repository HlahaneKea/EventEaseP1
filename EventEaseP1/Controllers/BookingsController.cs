using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEaseP1.Models;

namespace EventEaseP1.Controllers
{
    public class BookingsController : Controller
    {
        private readonly Poepart1Context _context;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(Poepart1Context context, ILogger<BookingsController> logger)
        {
            _context = context;
            _logger = logger;
        }


        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var poepart1Context = _context.Bookings.Include(b => b.Event).Include(b => b.Venue);
            return View(await poepart1Context.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }
        // GET: Bookings/Create
        // GET: Bookings/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                // Get events with their venues
                var events = await _context.Eventsses
                    .Include(e => e.Venue)
                    .Select(e => new
                    {
                        EventId = e.EventId,
                        DisplayText = $"{e.Name} - {e.EventDate:d}",
                        VenueId = e.VenueId,
                        VenueName = e.Venue.Name + " (" + e.Venue.Location + ")",
                        EventDate = e.EventDate.ToString("yyyy-MM-dd")
                    })
                    .ToListAsync();

                // Store both events and their details in ViewBag
                ViewBag.EventId = events.Select(e => new SelectListItem
                {
                    Value = e.EventId.ToString(),
                    Text = e.DisplayText
                });

                // Store event details for JavaScript
                ViewBag.EventDetails = events;

                return View(new Booking { BookingDate = DateOnly.FromDateTime(DateTime.Today) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Create GET action: {ex.Message}");
                return Problem("Error loading form data");
            }
        }
        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int UserId, int EventId, DateTime BookingDate, int VenueId)
        {
            try
            {
                _logger.LogInformation($"Attempting to create booking: UserId={UserId}, EventId={EventId}, Date={BookingDate}, VenueId={VenueId}");

                // Check for existing booking by the same user for the same event
                var existingBooking = await _context.Bookings
                    .AnyAsync(b => b.UserId == UserId &&
                                  b.EventId == EventId);

                if (existingBooking)
                {
                    ModelState.AddModelError("", "You have already booked this event.");
                    await ReloadFormData();
                    return View();
                }

                // Get the event to verify it exists
                var selectedEvent = await _context.Eventsses
                    .Include(e => e.Venue)
                    .FirstOrDefaultAsync(e => e.EventId == EventId);

                if (selectedEvent == null)
                {
                    ModelState.AddModelError("EventId", "Selected event not found");
                    await ReloadFormData();
                    return View();
                }

                // Convert the booking date
                var bookingDateOnly = DateOnly.FromDateTime(BookingDate);

                // Create the booking
                var booking = new Booking
                {
                    UserId = UserId,
                    EventId = EventId,
                    BookingDate = bookingDateOnly,
                    VenueId = VenueId
                };

                try
                {
                    _logger.LogInformation($"Attempting to save booking: {System.Text.Json.JsonSerializer.Serialize(booking)}");
                    _context.Bookings.Add(booking);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Successfully created booking with ID: {booking.BookingId}");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    var innerException = ex.InnerException?.Message ?? "No inner exception";
                    _logger.LogError($"Database error details: {ex.Message}, Inner exception: {innerException}");

                    ModelState.AddModelError("", "Unable to create booking. It may be a duplicate booking.");
                    await ReloadFormData();
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"General error in Create POST action: {ex.Message}");
                ModelState.AddModelError("", $"An unexpected error occurred: {ex.Message}");
                await ReloadFormData();
                return View();
            }
        }

        private async Task ReloadFormData()
        {
            try
            {
                var events = await _context.Eventsses
                    .Include(e => e.Venue)
                    .Select(e => new
                    {
                        EventId = e.EventId,
                        DisplayText = $"{e.Name} - {e.EventDate:d}",
                        VenueId = e.VenueId,
                        VenueName = e.Venue.Name + " (" + e.Venue.Location + ")",
                        EventDate = e.EventDate.ToString("yyyy-MM-dd")
                    })
                    .ToListAsync();

                ViewBag.EventId = events.Select(e => new SelectListItem
                {
                    Value = e.EventId.ToString(),
                    Text = e.DisplayText
                });

                ViewBag.EventDetails = events;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error reloading form data: {ex.Message}");
                ViewBag.EventId = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "-- Select Event --" }
        };
                ViewBag.EventDetails = new List<object>();
            }
        }
        // Helper method to reload the form with data

        private async Task<IActionResult> ReloadForm(int userId, int eventId, DateTime bookingDate)
        {
            var events = await _context.Eventsses
                .Select(e => new SelectListItem
                {
                    Value = e.EventId.ToString(),
                    Text = $"{e.Name} - {e.EventDate:d}"
                })
                .ToListAsync();

            ViewBag.EventId = events;

            return View(new Booking
            {
                UserId = userId,
                EventId = eventId,
                BookingDate = DateOnly.FromDateTime(bookingDate)
            });
        }
        [HttpGet]
        
        public async Task<IActionResult> GetEventVenue(int eventId)
        {
            var eventDetails = await _context.Eventsses
                .Include(e => e.Venue)
                .Where(e => e.EventId == eventId)
                .Select(e => new {
                    id = e.VenueId,
                    name = e.Venue.Name + " - " + e.Venue.Location
                })
                .FirstOrDefaultAsync();

            if (eventDetails == null)
                return NotFound();

            return Json(eventDetails);
        }
        [HttpGet]
        [Route("GetEventDetails/{eventId}")]  // Add this line
        public async Task<IActionResult> GetEventDetails(int eventId)
        {
            try
            {
                _logger.LogInformation($"Getting details for event {eventId}");

                // First verify the event exists with its venue
                var eventExists = await _context.Eventsses
                    .Include(e => e.Venue)
                    .AnyAsync(e => e.EventId == eventId);

                if (!eventExists)
                {
                    _logger.LogWarning($"Event {eventId} or its venue not found");
                    return NotFound($"Event {eventId} not found");
                }

                var eventDetails = await _context.Eventsses
                    .Include(e => e.Venue)
                    .Where(e => e.EventId == eventId)
                    .Select(e => new
                    {
                        venueId = e.VenueId,
                        venueName = e.Venue.Name + " (" + e.Venue.Location + ")",
                        eventDate = e.EventDate.ToString("yyyy-MM-dd"),
                        eventName = e.Name
                    })
                    .FirstOrDefaultAsync();

                _logger.LogInformation($"Successfully retrieved details for event {eventId}");
                return Json(eventDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting event {eventId} details: {ex.Message}", ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet]
        [Route("TestEventDetails/{eventId}")]
        public async Task<IActionResult> TestEventDetails(int eventId)
        {
            try
            {
                var eventInfo = await _context.Eventsses
                    .Include(e => e.Venue)
                    .Where(e => e.EventId == eventId)
                    .Select(e => new
                    {
                        eventId = e.EventId,
                        eventName = e.Name,
                        venueId = e.VenueId,
                        venueName = e.Venue != null ? e.Venue.Name : "No venue",
                        hasVenue = e.Venue != null
                    })
                    .FirstOrDefaultAsync();

                return Json(new
                {
                    event_found = eventInfo != null,
                    event_details = eventInfo,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    error = ex.Message,
                    stack_trace = ex.StackTrace,
                    timestamp = DateTime.Now
                });
            }
        }

        private async Task<bool> IsVenueDoubleBooked(int venueId, DateOnly bookingDate, int? excludeBookingId = null)
        {
            var query = _context.Bookings
                .Where(b => b.VenueId == venueId && b.BookingDate == bookingDate);

            if (excludeBookingId.HasValue)
            {
                query = query.Where(b => b.BookingId != excludeBookingId.Value);
            }

            return await query.AnyAsync();
        }
        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Eventsses, "EventId", "EventId", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", booking.VenueId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,UserId,EventId,BookingDate,VenueId")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
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
            ViewData["EventId"] = new SelectList(_context.Eventsses, "EventId", "EventId", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", booking.VenueId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);


        }

    }
}
