using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Передача контекста следующему middleware в конвейере
            }
            catch (Exception ex)
            {
                // Обработка исключения
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            // Устанавливаем код состояния
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            // Создаем объект ErrorDetails
            var errorDetails = new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error from the custom middleware.",
                ExceptionMessage = ex.Message
            };

            // Преобразуем объект ErrorDetails в строку в формате JSON
            var jsonErrorDetails = JsonSerializer.Serialize(errorDetails);

            // Отправляем детали об ошибке клиенту
            await context.Response.WriteAsync(jsonErrorDetails);
        }
    }

    // Модель для деталей об ошибке
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string ExceptionMessage { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}