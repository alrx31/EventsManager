using EventManagement.Application.Models;
using EventManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Infrastructure.Persistence;

public class ApplicationDbContext:DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
    {
    }
    
    // Tables
    
    public DbSet<Event> Events { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<EventParticipant> EventParticipants { get; set; }
    public DbSet<ExtendedIdentityUser> ExtendedIdentityUsers { get; set; }
 
    
    // Fluent API
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventParticipant>()
            .HasOne(ep => ep.Event)
            .WithMany(e => e.EventParticipants)
            .HasForeignKey(ep => ep.EventId);
        modelBuilder.Entity<EventParticipant>()
            .HasOne(ep => ep.Participant)
            .WithMany(p => p.EventParticipants)
            .HasForeignKey(ep => ep.ParticipantId);
        // one to many
        modelBuilder.Entity<ExtendedIdentityUser>()
            .HasOne(ex=>ex.Participant)
            .WithMany(p=>p.IdentityUsers)
            .HasForeignKey(e=>e.ParticipantId);
    }
}