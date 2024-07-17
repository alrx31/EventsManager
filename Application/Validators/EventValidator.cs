using EventManagement.Application.Models;
using EventManagement.Domain.Entities;
using FluentValidation;

namespace EventManagement.Application.Validators;

public class EventDTOValidator:AbstractValidator<EventDTO>
{
    public EventDTOValidator()
    {
        RuleFor(e=>e.Name).NotNull().NotEmpty().WithMessage("Name is required");
        RuleFor(e=>e.Description).NotNull().NotEmpty().WithMessage("Description is required");
        RuleFor(e=>e.Date).NotNull().NotEmpty().WithMessage("Date is required");
        RuleFor(e=>e.Location).NotNull().NotEmpty().WithMessage("Location is required");
        RuleFor(e=>e.Category).NotNull().NotEmpty().WithMessage("Category is required");
        RuleFor(e=>e.MaxParticipants).NotNull().NotEmpty().WithMessage("MaxParticipants is required");
        RuleFor(e=>e.ImageData).NotNull().NotEmpty().WithMessage("Image is required");
    }
}
