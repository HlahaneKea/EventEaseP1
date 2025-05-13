using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEaseP1.Models;

[Table("Eventss")]
public partial class Eventss
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("EventID")]
    public int EventId { get; set; }

    [Required(ErrorMessage = "Event name is required")]
    [Column("Name")]
    [StringLength(250, ErrorMessage = "Name cannot be longer than 250 characters")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Event date is required")]
    [Column("EventDate", TypeName = "date")]
    public DateTime EventDate { get; set; }
    [Required(ErrorMessage = "Venue is required")]
    [Column("VenueID")]
    public int VenueId { get; set; }

    [Column("Description")]
    public string? Description { get; set; }

    [Column("ImageUrl")]
    public string? ImageUrl { get; set; }

    [ForeignKey("VenueId")]
    public virtual Venue Venue { get; set; } = null!;

    [NotMapped]
    public IFormFile ImageFile { get; set; }


    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}