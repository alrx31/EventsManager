using EventManagement.Domain.Entities;
using FluentValidation;

namespace EventManagement.Application.Validators;

public class ParticipantValidator:AbstractValidator<Participant>
{
    public ParticipantValidator()
    {
        RuleFor(p=>p.FirstName).NotEmpty().WithMessage("First Name is required");
        RuleFor(p=>p.LastName).NotEmpty().WithMessage("Last Name is required");
        RuleFor(p=>p.Email).NotEmpty().WithMessage("Email is required");
        RuleFor(p=>p.Email).EmailAddress().WithMessage("Email is not valid");
        RuleFor(p=>p.BirthDate).NotEmpty().WithMessage("Birth Date is required");   
    }
}