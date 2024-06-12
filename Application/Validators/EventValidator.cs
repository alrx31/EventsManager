using EventManagement.Domain.Entities;
using FluentValidation;

namespace EventManagement.Application.Validators;

public class EventValidator:AbstractValidator<Event>
{
    public EventValidator()
    {
        RuleFor(e=>e.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(e=>e.Description).NotEmpty().WithMessage("Description is required");
        RuleFor(e=>e.Date).NotEmpty().WithMessage("Date is required");
        RuleFor(e=>e.Location).NotEmpty().WithMessage("Location is required");
        RuleFor(e=>e.Category).NotEmpty().WithMessage("Category is required");
        RuleFor(e=>e.MaxParticipants).NotEmpty().WithMessage("MaxParticipants is required");
    }
}