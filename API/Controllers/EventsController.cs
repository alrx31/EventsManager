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

        //1. Получение списка всех событий; c пагинацией;+
        [HttpGet("page={page}&pageSize={pageSize}")]

        public async Task<IActionResult> GetAllEvents([FromRoute] int page,int pageSize)
        {
            var events = await _eventService.GetAllEventsAsync(page,pageSize);
            if (events == null)
                return NotFound();

            return Ok(events);
        }

        //2. Получение определенного события по его Id;+
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventById = await _eventService.GetEventByIdAsyncRequest(id);
            return Ok(eventById);
        }

        //3. Получение события по его названию; +

        [HttpGet("event/{name}")]
        public async Task<IActionResult> GetEventByName(string name)
        {
            var eventByName = await _eventService.GetEventByNameAsync(name);

            return Ok(eventByName);
        }

        //4. Добавление нового события;+

        [HttpPost("create-event")]
        public async Task<IActionResult> AddEvent([FromForm] EventDTO newEvent)
        {
            // fluent validation
            Console.WriteLine(newEvent);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _eventService.AddEventAsync(newEvent);
            return Ok();
        }

        // 5. Изменение информации о существующем событии;
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromForm] EventDTO updatedEvent)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _eventService.UpdateEventAsync(id, updatedEvent);
            
            // TODO:оповещение всех пользователей о изменении события
            
            return Ok();
        }

        //6. Удаление события; +

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            await _eventService.DeleteEventAsync(id);
            return Ok();
        }

        //7. Получение списка событий по определенным критериям (по дате, месту проведения, категории события) +

        [HttpPost("filter&page={page}&pageSize={pageSize}")]
        public async Task<IActionResult> FilterEvents([FromBody] EventCriteria criteria,int page,int pageSize)
        {
            if (criteria == null)
                return BadRequest("Criteria is null");

            var events = await _eventService.GetEventsByCriteriaAsync(criteria,page,pageSize);
            return Ok(events);
        }

        //8. Возможность добавления изображений к событиям и их хранение. +
        // включена в тип Event   

        // получение списка событий пользователя
        [HttpGet("user-events/{UserId}")]
        public async Task<IActionResult> GetEventsFromUser(int UserId)
        {
            if (UserId < 1) throw new Exception("Invalid User Id");
            var events = await _eventService.getEventsByUserId(UserId);
            return Ok(events);
        }

        // поиск по дате или названию
        [HttpPost("search&page={page}&pageSize={pageSize}")]
        public async Task<IActionResult> SearchEvents([FromBody] SearchDTO model,[FromRoute] int page,int pageSize)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            if(model.Date == null && string.IsNullOrEmpty(model.Name))
                return BadRequest("Date or Name is required");
            var events = await _eventService.SearchEvents(model,page,pageSize);
            return Ok(events);
        }

        // пагинация при поиске
        [HttpPost("search/count")]
        public async Task<IActionResult> getCountEventsSearch([FromBody] SearchDTO model)
        {
            if (!ModelState.IsValid) return BadRequest();
            var count = await _eventService.GetCountEventsSearch(model);
            if (count > -1)
            {
                return Ok(count);
            }
            return BadRequest();
        }
        
        [HttpGet("count")]
        public async Task<IActionResult> GetCountEvents()
        {
            var count = await _eventService.GetCountEvents();
            if (count > -1)
            {
                return Ok(count);
            }
            return BadRequest();
        }

        [HttpPost("filter/count")]
        public async Task<IActionResult> getCountEventsFilter([FromBody] EventCriteria model)
        {
            if (!ModelState.IsValid) return BadRequest();
            var count = await _eventService.GetCountEventsFilter(model);
            if (count > -1)
            {
                return Ok(count);
            }
            return BadRequest();
        }
    }
}
