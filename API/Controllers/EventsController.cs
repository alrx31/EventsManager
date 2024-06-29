using EventManagement.Application.Models;
using EventManagement.Application.Services;
using EventManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
        }

        //1. Получение списка всех событий; 
        [HttpGet("events/{page}")]
        
        public async Task<IActionResult> GetAllEvents(int page)
        {
            var events = await _eventService.GetAllEventsAsync(page);
            if (events == null)
                return NotFound();

            return Ok(events);
        }
        //2. Получение определенного события по его Id;+
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventById = await _eventService.GetEventByIdAsync(id);
            if (eventById == null)
                return NotFound();

            return Ok(eventById);
        }
        
        //3. Получение события по его названию; +

        [HttpGet("event/{name}")]
        public async Task<IActionResult> GetEventByName(string name)
        {
            var eventByName = await _eventService.GetEventByNameAsync(name);
            if (eventByName == null)
                return NotFound();

            return Ok(eventByName);
        }
        
        //4. Добавление нового события;+

        [HttpPost]
        public async Task<IActionResult> AddEvent([FromForm] EventDTO newEvent)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _eventService.AddEventAsync(newEvent);
            return Ok();
        }
        
        // 5. Изменение информации о существующем событии;
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateEvent(int id, [FromForm] EventDTO updatedEvent)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _eventService.UpdateEventAsync(id,updatedEvent);
            return Ok();
        }
        
        //6. Удаление события; +

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            await _eventService.DeleteEventAsync(id);
            return Ok();
        }

        //7. Получение списка событий по определенным критериям (по дате, месту проведения, категории события) +
        
        [HttpPost("filter")]
        [Authorize]
        public async Task<IActionResult> FilterEvents([FromBody] EventCriteria criteria)
        {
            if (criteria == null)
                return BadRequest("Criteria is null");

            var events = await _eventService.GetEventsByCriteriaAsync(criteria);
            return Ok(events);
        }
        
        
        //8. Возможность добавления изображений к событиям и их хранение. +
        // включена в тип Event        
    }
}
