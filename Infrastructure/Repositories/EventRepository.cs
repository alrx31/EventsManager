using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using AutoMapper;
using EventManagement.Application.Models;
using EventManagement.Domain;
using EventManagement.Domain.Entities;
using EventManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;



namespace EventManagement.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public EventRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }


        public async Task<IEnumerable<EventRequest>> GetAllEventsAsync(int page, int pageSize)
        {
            var datas = await _dbContext.Events.ToListAsync();
            return datas.Skip((page - 1) * pageSize).Take(pageSize).Select(e => _mapper.Map<EventRequest>(e));
        }

        public async Task<Event> GetEventByIdAsync(int id)
        {
            return await _dbContext.Events.FindAsync(id);
        }

        public async Task<EventRequest> GetEventByIdAsyncRequest(int id)
        {

            var e = await _dbContext.Events.FindAsync(id);
            return _mapper.Map<EventRequest>(e); ;

        }

        public async Task<Event> GetEventByNameAsync(string name)
        {
            return await _dbContext.Events.FirstOrDefaultAsync(e => e.Name == name);

        }

        public async Task AddEventAsync(EventDTO newEvent)
        {

            var newEventEntity = _mapper.Map<Event>(newEvent);
            newEventEntity.EventParticipants = new List<EventParticipant>();

            if (newEvent.ImageData != null && newEvent.ImageData.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await newEvent.ImageData.CopyToAsync(memoryStream);

                if (memoryStream.Length > 0)
                {
                    newEventEntity.ImageData = memoryStream.ToArray();
                }
                else
                {
                    Console.WriteLine("Ошибка: Пустой поток данных.");
                }
            }

            await _dbContext.Events.AddAsync(newEventEntity);

        }

        public async Task UpdateEventAsync(int eventId, EventDTO updatedEvent)
        {


            var eventToUpdate = await GetEventByIdAsync(eventId);

            // update all field
            eventToUpdate.Name = updatedEvent.Name;
            eventToUpdate.Description = updatedEvent.Description;
            eventToUpdate.Location = updatedEvent.Location;
            eventToUpdate.Category = updatedEvent.Category;
            eventToUpdate.Date = (DateTime)updatedEvent.Date;
            eventToUpdate.MaxParticipants = (int)updatedEvent.MaxParticipants;


            if (updatedEvent.ImageData.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await updatedEvent.ImageData.CopyToAsync(memoryStream);

                if (memoryStream.Length > 0)
                {
                    eventToUpdate.ImageData = memoryStream.ToArray();
                }
                else
                {
                    Console.WriteLine("Ошибка: Пустой поток данных.");
                }
            }

            _dbContext.Events.Update(eventToUpdate);
        }

        public async Task DeleteEventAsync(int id)
        {
            var eventToDelete = await GetEventByIdAsync(id);
            _dbContext.Events.Remove(eventToDelete);
        }


        public async Task<List<EventRequest>> GetEventsByCriteriaAsync(EventCriteria criteria, int page, int pageSize)
        {
            IQueryable<Event> query = _dbContext.Events;


            if (!string.IsNullOrEmpty(criteria.Location))
            {
                query = query.Where(e => e.Location == criteria.Location);
            }

            if (!string.IsNullOrEmpty(criteria.Category))
            {
                query = query.Where(e => e.Category == criteria.Category);
            }

            return await query.Skip((page - 1) * pageSize).Take(pageSize).Select(e => _mapper.Map<EventRequest>(e)).ToListAsync();
        }

        public async Task<List<EventRequest>> getEventsByUserId(int id)
        {
            return await _dbContext.Events.Where(e => e.EventParticipants.Any(ep => ep.ParticipantId == id))
                .Select(e => _mapper.Map<EventRequest>(e)).ToListAsync();
        }

        public async Task<List<EventRequest>> SearchEvents(SearchDTO model, int page, int pageSize)
        {
            var events = _dbContext.Events.Where(e => e.Date == model.Date.ToUniversalTime() || (!String.IsNullOrEmpty(model.Name) && e.Name.Contains(model.Name)));
            return await events.Skip((page - 1) * pageSize).Take(pageSize).Select(e => _mapper.Map<EventRequest>(e)).ToListAsync();
        }

        public async Task<int> GetCountEvents()
        {
            return await _dbContext.Events.CountAsync();
        }

        public async Task<int> GetCountEventsSearch(SearchDTO model)
        {
            return await _dbContext.Events.CountAsync(e => e.Date == model.Date.ToUniversalTime() || (!String.IsNullOrEmpty(model.Name) && e.Name.Contains(model.Name)));
        }

        public async Task<int> GetCountEventsFilter(EventCriteria model)
        {
            return await _dbContext.Events.CountAsync(e => (string.IsNullOrEmpty(model.Location) || e.Location == model.Location) && (string.IsNullOrEmpty(model.Category) || e.Category == model.Category));
        }
    }
}