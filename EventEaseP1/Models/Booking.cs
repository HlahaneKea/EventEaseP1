using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EventEaseP1.Models;

public partial class Booking
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("BookingID")]
    public int BookingId { get; set; }

    [Required(ErrorMessage = "User ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid User ID")]
    public int UserId { get; set; }


    [Required(ErrorMessage = "Please select an event")]
    [Display(Name = "Event")]
    public int EventId { get; set; }

    [Required(ErrorMessage = "Booking date is required")]
    [Display(Name = "Booking Date")]
    public DateOnly BookingDate { get; set; }

    [Required(ErrorMessage = "Please select a venue")]
    [Display(Name = "Venue")]
    public int VenueId { get; set; }

    [ForeignKey("EventId")]
    public virtual Eventss Event { get; set; } = null!;

    [ForeignKey("VenueId")]
    public virtual Venue Venue { get; set; } = null!;
}