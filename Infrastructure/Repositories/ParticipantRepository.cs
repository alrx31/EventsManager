using System;
using EventManagement.Application.Models;
using EventManagement.Domain.Entities;
using EventManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace EventManagement.Infrastructure.Repositories
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        
        public ParticipantRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task RegisterParticipantToEventAsync(int eventId, int participantId)
        {
            await _context.Events.FindAsync(eventId);
            await _context.Participants.FindAsync(participantId);
            _context.EventParticipants.Add(new EventParticipant
            {
                EventId = eventId,
                ParticipantId = participantId
            });
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
            return await _context.Participants.FirstOrDefaultAsync(p => p.Id ==id);
        }

        public async Task CancelRegistrationAsync(int eventId, int participantId)
        {
            var eventParticipant = await _context.EventParticipants
                .Where(ep => ep.EventId == eventId && ep.ParticipantId == participantId)
                .FirstOrDefaultAsync();
            
            _context.EventParticipants.Remove(eventParticipant);
        }
        
        // TODO: email sending logic
        public async Task SendEmailToParticipantAsync(int eventId, int participantId, string message)
        {
            // Логика отправки электронной почты участнику
            throw new NotImplementedException("send email method  not implemented");
        }
        
        
        
        
        
        public async Task RegisterParticipantAsync(ParticipantRegisterDTO user)
        {
            var participant = _mapper.Map<Participant>(user);
                
            // назначение админа по имени
            participant.IsAdmin = participant.FirstName == "admin";
            
            
            participant.EventParticipants = new List<EventParticipant>();
            participant.Password = GetHash(user.Password);
            await _context.Participants.AddAsync(participant);
        }
        

        public async Task<LoginModel> GetParticipantByEmailAsync(string email)
        {
            var user = await _context.Participants
                .FirstOrDefaultAsync(p => p.Email == email);
            
            if (user == null)
            {
                return null;
            }
            
            return new LoginModel
            {
                Email = user.Email,
                Password = user.Password
            };
        }

        public async Task<int> GetParticipantIdByEmailAsync(string email)
        {
            var user = await _context.Participants.FirstOrDefaultAsync(p => p.Email == email);
            if (user == null)
            {
                return 0;
            }
            return user.Id;
        } 

        public Task<bool> CheckPasswordAsync(LoginModel user, string password)
        {
            return Task.FromResult(user.Password == GetHash(password));
        }

        public async Task<ExtendedIdentityUser> getExtendedIdentityUserByEmailAsync(string email)
        {
            var user = await _context.ExtendedIdentityUsers
                .FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }

        public async Task UpdateRefreshTokenAsync(ExtendedIdentityUser user)
        {
            var user1 = await _context.ExtendedIdentityUsers
                .FirstOrDefaultAsync(u => u.Email == user.Email);
            _context.ExtendedIdentityUsers.Update(user);
        }

        public async Task AddRefreshTokenField(ParticipantRegisterDTO user)
        {
            var newToken = new ExtendedIdentityUser
            {
                Email = user.Email,
                RefreshToken = "",
                RefreshTokenExpiryTime = DateTime.UtcNow,
                ParticipantId = await _context.Participants
                    .Where(p => p.Email == user.Email)
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync(),
                Participant = null
            };
            await _context.ExtendedIdentityUsers.AddAsync(newToken);
        }


        public async Task CanselRefreshToken(int userId)
        {
            var user = await _context.ExtendedIdentityUsers
                .FirstOrDefaultAsync(u => u.ParticipantId == userId);
            user!.RefreshTokenExpiryTime = DateTime.UtcNow;
        }


        private string GetHash(string pass)
        {
            var data = System.Text.Encoding.ASCII.GetBytes(pass);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
            return Encoding.ASCII.GetString(data);
        }

        
        
    }
}
