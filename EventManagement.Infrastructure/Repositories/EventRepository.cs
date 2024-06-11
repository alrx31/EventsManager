using EventManagement.Application.Models;
using EventManagement.Domain.Entities;
using EventManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public EventRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _dbContext.Events.ToListAsync();
        }

        public async Task<Event> GetEventByIdAsync(int id)
        {
            return await _dbContext.Events.FindAsync(id);
        }

        public async Task<Event> GetEventByNameAsync(string name)
        {
            return await _dbContext.Events.FirstOrDefaultAsync(e => e.Name == name);
        }

        public async Task AddEventAsync(Event newEvent)
        {
            _dbContext.Events.Add(newEvent);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateEventAsync(Event updatedEvent)
        {
            var eventToUpdate = await _dbContext.Events.FindAsync(updatedEvent.Id);
            eventToUpdate.Name = updatedEvent.Name;
            eventToUpdate.Date = updatedEvent.Date;
            eventToUpdate.Location = updatedEvent.Location;
            eventToUpdate.Category = updatedEvent.Category;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteEventAsync(int id)
        {
            var eventToDelete = await _dbContext.Events.FindAsync(id);
            _dbContext.Events.Remove(eventToDelete);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByCriteriaAsync(EventCriteria criteria)
        {
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
