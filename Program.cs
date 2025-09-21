using System.Text;
using EventManagement.Application;
using EventManagement.Application.Services;
using EventManagement.Application.Validators;
using EventManagement.Domain;
using EventManagement.Infrastructure;
using EventManagement.Infrastructure.Persistence;
using EventManagement.Infrastructure.Repositories;
using EventManagement.Middlewares;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<EventDTOValidator>());
builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<ParticipantDTOValidator>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IParticipantService, ParticipantService>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IParticipantRepository, ParticipantRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<ApplicationDbContext>();

const int maxRetries = 10;
int retryCount = 0;

while (true)
{
    try
    {
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
        break;
    }
    catch (Exception ex)
    {
        retryCount++;
        if (retryCount >= maxRetries)
        {
            throw new Exception("Exceeded max retry attempts to connect to DB", ex);
        }

        Console.WriteLine($"Retrying DB connection ({retryCount}/{maxRetries})...");
        Thread.Sleep(2000);
    }
}

app.UseCors("AllowLocalhost3000");  // Применяем правильную политику CORS

if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseStaticFiles();
app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();
