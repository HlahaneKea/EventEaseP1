using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEaseP1.Models;

public partial class Venue
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("VenueID")]
    public int VenueId { get; set; }

    [Required(ErrorMessage = "Venue name is required")]
   // [Display(Name = "Venue Name")]
    [StringLength(250)]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Location is required")]
    [StringLength(250)]
    public string Location { get; set; } = null!;

    [Required(ErrorMessage = "Capacity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
    public int Capacity { get; set; }

    public string? ImageUrl { get; set; }

    [NotMapped]
    public IFormFile? ImageFile { get; set; }

    public bool IsAvailable { get; set; } = true;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public virtual ICollection<Eventss> Eventsses { get; set; } = new List<Eventss>();
}