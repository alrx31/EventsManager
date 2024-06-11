using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventManagement.Application.Models;
using EventManagement.Domain.Entities;
using EventManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;

        public EventRepository(ApplicationDbContext dbContext, IUnitOfWork unitOfWork)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _dbContext.Events.ToListAsync();
        }

        public async Task<Event> GetEventByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Invalid event ID");
            }

            return await _dbContext.Events.FindAsync(id);
        }

        public async Task<Event> GetEventByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name), "Event name cannot be null or empty");
            }

            return await _dbContext.Events.FirstOrDefaultAsync(e => e.Name == name);
        }

        public async Task AddEventAsync(Event newEvent)
        {
            if (newEvent == null)
            {
                throw new ArgumentNullException(nameof(newEvent), "Event is null");
            }

            await _dbContext.Events.AddAsync(newEvent);
            await _unitOfWork.CompleteAsync(); // Сохраняем изменения в базе данных
        }

        public async Task UpdateEventAsync(Event updatedEvent)
        {
            if (updatedEvent == null)
            {
                throw new ArgumentNullException(nameof(updatedEvent), "Event is null");
            }

            _dbContext.Events.Update(updatedEvent);
            await _unitOfWork.CompleteAsync(); // Сохраняем изменения в базе данных
        }

        public async Task DeleteEventAsync(int id)
        {
            var eventToDelete = await GetEventByIdAsync(id);
            if (eventToDelete == null)
            {
                throw new InvalidOperationException("Event not found");
            }

            _dbContext.Events.Remove(eventToDelete);
            await _unitOfWork.CompleteAsync(); // Сохраняем изменения в базе данных
        }

        public async Task<IEnumerable<Event>> GetEventsByCriteriaAsync(EventCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria), "Criteria is null");
            }

            IQueryable<Event> query = _dbContext.Events;

            if (criteria.Date.HasValue)
            {
                query = query.Where(e => e.Date == criteria.Date.Value);
            }

            if (!string.IsNullOrEmpty(criteria.Location))
            {
                query = query.Where(e => e.Location == criteria.Location);
            }

            if (!string.IsNullOrEmpty(criteria.Category))
            {
                query = query.Where(e => e.Category == criteria.Category);
            }

            return await query.ToListAsync();
        }
        
        
    }
}
