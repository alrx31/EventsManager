using EventManagement.Application.Models;
using EventManagement.Domain.Entities;
using EventManagement.Infrastructure.Repositories;
using FluentValidation;

namespace EventManagement.Application.Validators;

public class ParticipantDTOValidator:AbstractValidator<ParticipantRegisterDTO>
{
    public ParticipantDTOValidator()
    {
        RuleFor(p=>p.FirstName).NotNull().NotEmpty().WithMessage("First Name is required");
        RuleFor(p=>p.LastName).NotNull().NotEmpty().WithMessage("Last Name is required");
        RuleFor(p=>p.Email).NotNull().NotEmpty().WithMessage("Email is required");
        RuleFor(p=>p.Email).EmailAddress().WithMessage("Email is not valid");
        RuleFor(p=>p.BirthDate).NotNull().NotEmpty().WithMessage("Birth Date is required");   
        RuleFor(p=>p.Password).NotNull().NotEmpty().WithMessage("Password is required");
    }
}