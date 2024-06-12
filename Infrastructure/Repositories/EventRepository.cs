using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EventManagement.Application.Models;
using EventManagement.Domain;
using EventManagement.Domain.Entities;
using EventManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;



namespace EventManagement.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public EventRepository(ApplicationDbContext dbContext, IUnitOfWork unitOfWork,IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
            var eventById = await _dbContext.Events.FindAsync(id);

            return eventById ?? throw new InvalidOperationException("Event not found");
        }

        public async Task<Event> GetEventByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name), "Event name cannot be null or empty");
            }
            var @return = await _dbContext.Events.FirstOrDefaultAsync(e => e.Name == name);
            return @return ?? throw new InvalidOperationException("Event not found");
        }

        public async Task AddEventAsync(EventDTO newEvent)
        {
            if (newEvent == null)
            {
                throw new ArgumentNullException(nameof(newEvent), "Event is null");
            }

            var newEventEntity = _mapper.Map<Event>(newEvent);
            var lastEventId = await GetLastEventId();
            newEventEntity.Id = lastEventId + 1;
            newEventEntity.EventParticipants = new List<EventParticipant>();

            if (newEvent.ImageData != null && newEvent.ImageData.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await newEvent.ImageData.CopyToAsync(memoryStream);

                // Проверяем, что memoryStream содержит данные
                if (memoryStream.Length > 0)
                {
                    newEventEntity.ImageData = memoryStream.ToArray();
                }
                else
                {
                    // Выводим сообщение об ошибке или выполняем другие действия
                    Console.WriteLine("Ошибка: Пустой поток данных.");
                }
            }

            await _dbContext.Events.AddAsync(newEventEntity);

            await _dbContext.SaveChangesAsync();
        }

        

        public async Task UpdateEventAsync(int eventId,EventDTO updatedEvent)
        {
            if (updatedEvent == null)
            {
                throw new ArgumentNullException(nameof(updatedEvent), "Event is null");
            }
            
            var eventToUpdate = await GetEventByIdAsync(eventId);
            
            if (eventToUpdate == null)
            {
                throw new InvalidOperationException("Event not found");
            }

            
            if(!String.IsNullOrEmpty(updatedEvent.Name))
            {
                eventToUpdate.Name = updatedEvent.Name;
            }
            if(!String.IsNullOrEmpty(updatedEvent.Description))
            {
                eventToUpdate.Description = updatedEvent.Description;
            }
            if(!String.IsNullOrEmpty(updatedEvent.Location))
            {
                eventToUpdate.Location = updatedEvent.Location;
            }
            if(!String.IsNullOrEmpty(updatedEvent.Category))
            {
                eventToUpdate.Category = updatedEvent.Category;
            }
            if(updatedEvent.Date != null)
            {
                eventToUpdate.Date = updatedEvent.Date ?? new DateTime();
            }
            if(updatedEvent.MaxParticipants != null)
            {
                eventToUpdate.MaxParticipants = updatedEvent.MaxParticipants ?? 0;
            }
            if (updatedEvent.ImageData != null && updatedEvent.ImageData.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await updatedEvent.ImageData.CopyToAsync(memoryStream);

                // Проверяем, что memoryStream содержит данные
                if (memoryStream.Length > 0)
                {
                    eventToUpdate.ImageData = memoryStream.ToArray();
                }
                else
                {
                    // Выводим сообщение об ошибке или выполняем другие действия
                    Console.WriteLine("Ошибка: Пустой поток данных.");
                }
            }
            
            _dbContext.Events.Update(eventToUpdate);
            await _dbContext.SaveChangesAsync(); // Сохраняем изменения в базе данных
        }

        public async Task DeleteEventAsync(int id)
        {
            var eventToDelete = await GetEventByIdAsync(id);
            if (eventToDelete == null)
            {
                throw new InvalidOperationException("Event not found");
            }

            _dbContext.Events.Remove(eventToDelete);
            await _dbContext.SaveChangesAsync(); // Сохраняем изменения в базе данных
        }

        public async Task<IEnumerable<Event>> GetEventsByCriteriaAsync(EventCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria), "Criteria is null");
            }

            IQueryable<Event> query = _dbContext.Events;

            /*if (criteria.Date.HasValue)
            {
                query = query.Where(e => e.Date == criteria.Date.Value);
            }*/

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
        
        
        
        private async Task<int> GetLastEventId()
        {
            return  await _dbContext.Events.AnyAsync() ? await _dbContext.Events.MaxAsync(e => e.Id) : 0;
        }
        
    }
}
