using EventManagement.Application.Models;
using EventManagement.Domain.Entities;
using EventManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManagement.Infrastructure.Repositories
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public ParticipantRepository(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task RegisterParticipantToEventAsync(int eventId, int participantId)
        {
            _context.EventParticipants.Add(new EventParticipant
            {
                EventId = eventId,
                ParticipantId = participantId
            });
            await _unitOfWork.CompleteAsync(); // Сохраняем изменения в базе данных через UnitOfWork
        }

        public async Task<IEnumerable<Participant>> GetParticipantsByEventIdAsync(int eventId)
        {
            return await _context.EventParticipants
                .Where(ep => ep.EventId == eventId)
                .Select(ep => ep.Participant)
                .ToListAsync();
        }

        public async Task<Participant> GetParticipantByIdAsync(int id)
        {
            return await _context.Participants.FindAsync(id);
        }

        public async Task CancelRegistrationAsync(int eventId, int participantId)
        {
            var eventParticipant = await _context.EventParticipants
                .Where(ep => ep.EventId == eventId && ep.ParticipantId == participantId)
                .FirstOrDefaultAsync();
            if (eventParticipant != null)
            {
                _context.EventParticipants.Remove(eventParticipant);
                await _unitOfWork.CompleteAsync(); // Сохраняем изменения в базе данных через UnitOfWork
            }
        }

        public async Task SendEmailToParticipantAsync(int eventId, int participantId, string message)
        {
            // Логика отправки электронной почты участнику
        }
    }
}
