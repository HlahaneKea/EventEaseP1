using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EventEaseP1.Models;

public partial class Poepart1Context : DbContext
{
    public Poepart1Context()
    {
    }

    public Poepart1Context(DbContextOptions<Poepart1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Eventss> Eventsses { get; set; }

    public virtual DbSet<Venue> Venues { get; set; }

    public virtual DbSet<EventType> EventTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configuration is handled through dependency injection in Program.cs
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__73951ACD5AD8848B");

           

            entity.HasOne(d => d.Event).WithMany(p => p.Bookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__EventI__3C69FB99");

            entity.HasOne(d => d.Venue).WithMany(p => p.Bookings).HasConstraintName("FK__Bookings__VenueI__47DBAE45");
        });

        modelBuilder.Entity<Eventss>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Eventss__7944C87065698F4E");

           

            entity.HasOne(d => d.Venue).WithMany(p => p.Eventsses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Eventss__VenueID__398D8EEE");
            entity.HasOne(d => d.EventType)
         .WithMany(p => p.Events)
         .OnDelete(DeleteBehavior.ClientSetNull)
         .HasConstraintName("FK__Eventss__EventTypeId");
        
    });

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.HasKey(e => e.VenueId).HasName("PK__Venues__3C57E5D2C536C794");

          // entity.Property(e => e.VenueId).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
