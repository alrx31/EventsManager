using EventManagement.Application.Models;
using EventManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ApplicationDbContextConfiguration : IEntityTypeConfiguration<Event>,
    IEntityTypeConfiguration<EventParticipant>,
    IEntityTypeConfiguration<ExtendedIdentityUser>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
    }

    public void Configure(EntityTypeBuilder<EventParticipant> builder)
    {
        builder.HasOne(ep => ep.Event)
            .WithMany(e => e.EventParticipants)
            .HasForeignKey(ep => ep.EventId);

        builder.HasOne(ep => ep.Participant)
            .WithMany(p => p.EventParticipants)
            .HasForeignKey(ep => ep.ParticipantId);
    }

    public void Configure(EntityTypeBuilder<ExtendedIdentityUser> builder)
    {
        builder.HasOne(ex => ex.Participant)
            .WithMany(p => p.IdentityUsers)
            .HasForeignKey(e => e.ParticipantId);
    }
}