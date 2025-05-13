using EventEaseP1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class BookingManagementController : Controller
{
    private readonly Poepart1Context _context;
    private readonly ILogger<BookingManagementController> _logger;

    public BookingManagementController(Poepart1Context context, ILogger<BookingManagementController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string searchString, string searchType)
    {
        try
        {
            var query = from b in _context.Bookings
                        join e in _context.Eventsses on b.EventId equals e.EventId
                        join v in _context.Venues on b.VenueId equals v.VenueId
                        select new BookingDetails
                        {
                            BookingId = b.BookingId,
                            UserId = b.UserId,
                            BookingDate = b.BookingDate,
                            EventId = e.EventId,
                            EventName = e.Name,
                            EventDate = e.EventDate,
                            EventDescription = e.Description,
                            VenueId = v.VenueId,
                            VenueName = v.Name,
                            VenueLocation = v.Location,
                            VenueCapacity = v.Capacity
                        };

            if (!string.IsNullOrEmpty(searchString))
            {
                switch (searchType)
                {
                    case "bookingId":
                        if (int.TryParse(searchString, out int bookingId))
                        {
                            query = query.Where(b => b.BookingId == bookingId);
                        }
                        break;
                    case "eventName":
                        query = query.Where(b => b.EventName.Contains(searchString));
                        break;
                }
            }

            var bookings = await query.ToListAsync();
            return View(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving bookings: {ex.Message}");
            TempData["Error"] = "Error loading bookings. Please try again.";
            return View(new List<BookingDetails>());
        }
    }
}