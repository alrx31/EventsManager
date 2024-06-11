using EventManagement.Domain.Entities;

namespace EventManagement.Application.Services
{
    public interface IParticipantService
    {
        // main
        Task RegisterParticipantToEventAsync(int eventId, int participantId);
        Task<IEnumerable<Participant>> GetParticipantsByEventIdAsync(int eventId);
        Task<Participant> GetParticipantByIdAsync(int id);
        Task CancelRegistrationAsync(int eventId, int participantId);
        Task SendEmailToParticipantAsync(int eventId, int participantId, string message);
    }
}
/*
 *Работа с событиями:
* 1. Получение списка всех событий;
2. Получение определенного события по его Id;
3. Получение события по его названию;
4. Добавление нового события;
5. Изменение информации о существующем событии;
6. Удаление события;
7. Получение списка событий по определенным критериям (по дате, месту 
проведения, категории события)
8. Возможность добавления изображений к событиям и их хранение.
Работа с участниками:
1. Регистрация участия пользователя в событии;
2. Получение списка участников события;
3. Получение определенного участника по его Id;
4. Отмена участия пользователя в событии;
5. *Отправка уведомлений участникам события о изменениях в событии 
(например, об изменении даты или места проведения) (будет плюсом)
 *
 * 
 */