using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EventManagement.Application.Models;
using EventManagement.Domain;
using EventManagement.Domain.Entities;
using EventManagement.Infrastructure.Persistence;
using EventManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Repositories;

public class ParticipantRepositoryTests
{
    private readonly ApplicationDbContext _context;
    private readonly IParticipantRepository _repository;
    private readonly IMapper _mapper;
    
    private DbContextOptions<ApplicationDbContext> _options;
    

    public ParticipantRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        _context = new ApplicationDbContext(_options);

        var config = new MapperConfiguration(c => c.AddProfile<AutoMapperProfile>());
        _mapper = config.CreateMapper();
        _repository = new ParticipantRepository(_context, _mapper);

        SeedDatabase(_context);
    }

    private void SeedDatabase(ApplicationDbContext _context)
    {
        _context.Participants.RemoveRange(_context.Participants);
        _context.SaveChanges();

        _context.Participants.Add(new Participant
        {
            Id = 1,
            FirstName = "John1",
            LastName = "Doe1",
            Email = "1",
            BirthDate = new DateTime(),
            RegistrationDate = new DateTime(),
            Password = Encoding.ASCII.GetString(new System.Security.Cryptography.SHA256Managed().ComputeHash( System.Text.Encoding.ASCII.GetBytes("as"))),
            IsAdmin = false
        });
        _context.SaveChanges();
        
        _context.Participants.Add(new Participant
        {
            Id = 2,
            FirstName = "John2",
            LastName = "Doe2",
            Email = "",
            BirthDate = new DateTime(),
            RegistrationDate = new DateTime(),
            Password = "",
            IsAdmin = false
        });

        _context.SaveChanges();
    }



    [Fact]
    public async Task RegisterParticipantToEventAsync_Success()
    {

        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        var _repository = new ParticipantRepository(_context,_mapper);

        _context.Events.Add(new Event
        {
            Id = 1,
            Name = "Event1",
            Description = "Description",
            Date = new DateTime(),
            Location = "Location",
            Category = "",
            MaxParticipants = 3,
            ImageData = new byte[2]
        });
        await _context.SaveChangesAsync();
        
        int eventId = 1;
        int participantId = 1;
        
        await _repository.RegisterParticipantToEventAsync(eventId, participantId);
        
        var eventParticipant = await _context.EventParticipants
            .Where(ep => ep.EventId == eventId && ep.ParticipantId == participantId)
            .FirstOrDefaultAsync();
        
        Assert.NotNull(eventParticipant);
    } 
    
    [Fact]
    public async Task RegisterParticipantToEventAsync_Fail_ParticipantNotFound()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        var _repository = new ParticipantRepository(_context,_mapper);
        
        int eventId = 1;
        int participantId = 3;
        
        await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.RegisterParticipantToEventAsync(eventId, participantId));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(-15)]
    public async Task GetParticipantsByEventIdAsync_Fail_Empty(int eventId)
    {
        var participants = await _repository.GetParticipantsByEventIdAsync(eventId);
        Assert.Empty(participants);
    }
    
    [Fact]
    public async Task GetParticipantsByEventIdAsync_Success()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        _context.Events.Add(new Event
        {
            Id = 3,
            Name = "Event1",
            Description = "Description",
            Date = new DateTime(),
            Location = "Location",
            Category = "",
            MaxParticipants = 3,
            ImageData = new byte[2]
        });
        _context.EventParticipants.Add(new EventParticipant
        {
            EventId = 3,
            ParticipantId = 1
        });
        _context.SaveChanges();
        
        var _repository = new ParticipantRepository(_context,_mapper);
        
        var participants = await _repository.GetParticipantsByEventIdAsync(3);
        Assert.NotEmpty(participants);
    }


    [Fact]
    public async Task GetParticipantByIdAsync_Success()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        var _repository = new ParticipantRepository(_context,_mapper);
        
        var res = await _repository.GetParticipantByIdAsync(1);
        
        Assert.NotNull(res);
        Assert.Equal(_context.Participants.FindAsync(1).Result.FirstName,res.FirstName);
    }


    [Fact]
    public async Task GetParticipantByIdAsync_Fail_notFound()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        var _repository = new ParticipantRepository(_context,_mapper);
        
        var res = await _repository.GetParticipantByIdAsync(3);
        
        Assert.Null(res);
    }


    [Fact]
    public async Task CancelRegistrationAsync_Success()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        _context.Events.Add(new Event
        {
            Id = 3,
            Name = "Event1",
            Description = "Description",
            Date = new DateTime(),
            Location = "Location",
            Category = "",
            MaxParticipants = 3,
            ImageData = new byte[2]
        });
        _context.EventParticipants.Add(new EventParticipant
        {
            EventId = 3,
            ParticipantId = 1
        });
        _context.SaveChanges();
        var _repository = new ParticipantRepository(_context,_mapper);
        
        await _repository.CancelRegistrationAsync(3, 1);
        
        var eventParticipant = await _context.EventParticipants
            .Where(ep => ep.EventId == 3 && ep.ParticipantId == 1)
            .FirstOrDefaultAsync();
        
        Assert.Null(eventParticipant);
    }



    [Fact]
    public async Task CancelRegistrationAsync_Fail_notFound()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        _context.Events.Add(new Event
        {
            Id = 3,
            Name = "Event1",
            Description = "Description",
            Date = new DateTime(),
            Location = "Location",
            Category = "",
            MaxParticipants = 3,
            ImageData = new byte[2]
        });
        _context.SaveChanges();
        var _repository = new ParticipantRepository(_context,_mapper);
        await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.CancelRegistrationAsync(3, 1));
    }


    [Fact]
    public async Task RegisterParticipantAsync_Success()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        var _repository = new ParticipantRepository(_context,_mapper);


        var newPart = new ParticipantRegisterDTO
        {
            FirstName = "test",
            LastName = "test",
            Email = "test",
            Password = "test",
            BirthDate = new DateTime(),
            RegistrationDate = new DateTime(),
        };
        
        await _repository.RegisterParticipantAsync(newPart);

        var part = await _context.Participants.FirstOrDefaultAsync(p => p.FirstName == newPart.FirstName && p.LastName == newPart.LastName);

        Assert.NotNull(part);
    }


    [Fact]
    public async Task LoginAsync_Success()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        var _repository = new ParticipantRepository(_context,_mapper);
        
        var loginModel = new LoginModel
        {
            Email = "1",
            Password = "as"
        };
        
        var res = await _repository.LoginAsync(loginModel);
        
        Assert.NotNull(res);
        Assert.Equal(_context.Participants.FirstOrDefaultAsync(p => p.Email == loginModel.Email).Result.FirstName,res.FirstName);
    }
    
    [Fact]
    public async Task LoginAsync_Fail_not_found()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        var _repository = new ParticipantRepository(_context,_mapper);
        
        var loginModel = new LoginModel
        {
            Email = "1",
            Password = "2"
        };
    
        await Assert.ThrowsAsync<Exception>(() => _repository.LoginAsync(loginModel));
    }


    [Fact]
    public async Task GetParticipantByEmailAsync_Success()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        var _repository = new ParticipantRepository(_context,_mapper);
        
        var res= await _repository.GetParticipantByEmailAsync("1");
        Assert.NotNull(res);
        Assert.Equal(_context.Participants.FirstOrDefaultAsync(p=>p.Email == "1").Result.Email ,res.Email);
        Assert.Equal(_context.Participants.FirstOrDefaultAsync(p => p.Email == "1").Result.Password,res.Password);
    }
    
    [Fact]
    public async Task GetParticipantByEmailAsync_Fail_ontFound()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        var _repository = new ParticipantRepository(_context,_mapper);
        
       await Assert.ThrowsAsync<Exception>(() => _repository.GetParticipantByEmailAsync("3"));
        
    }



    [Fact]
    public async Task getExtendedIdentityUserByEmailAsync_Success()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        var _repository = new ParticipantRepository(_context,_mapper);
        await _repository.AddRefreshTokenField(new ParticipantRegisterDTO
        {
            Email = "test",
            FirstName = "test",
            LastName = "test",
            Password = "test",
            BirthDate = new DateTime(),
            RegistrationDate = new DateTime()
        });
        
        var res = await _repository.getExtendedIdentityUserByEmailAsync("test");
        Assert.NotNull(res);
    }
    
    
    [Fact]
    public async Task getExtendedIdentityUserByEmailAsync_Fail_invalidEmail()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        var _repository = new ParticipantRepository(_context,_mapper);
        
        
        await Assert.ThrowsAsync<Exception>(()=> _repository.getExtendedIdentityUserByEmailAsync(""));
    }
    
    
    [Fact]
    public async Task getExtendedIdentityUserByEmailAsync_Fail_userNotFound()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        var _repository = new ParticipantRepository(_context,_mapper);
        
        
        await Assert.ThrowsAsync<Exception>(()=> _repository.getExtendedIdentityUserByEmailAsync("asdasd"));
    }
    
    
    
    [Fact]
    public async Task UpdateRefreshTokenAsync_Success()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        var part = new ParticipantRegisterDTO
        {
            Email = "test",
            FirstName = "test",
            LastName = "test",
            Password = "test",
            BirthDate = new DateTime(),
            RegistrationDate = new DateTime()
        };
        var _repository = new ParticipantRepository(_context,_mapper);
        
        await _repository.AddRefreshTokenField(part);
        
        var user = await _repository.getExtendedIdentityUserByEmailAsync("test");
        await _repository.UpdateRefreshTokenAsync(user);
        
        var user1 = await _context.ExtendedIdentityUsers.FirstOrDefaultAsync(u => u.Email == user.Email);
        
        Assert.NotNull(user1);
        Assert.Equal(user.RefreshToken,user1.RefreshToken);
        Assert.Equal(user.RefreshTokenExpiryTime,user1.RefreshTokenExpiryTime);
        
    }
    
    
    [Fact]
    public async Task UpdateRefreshTokenAsync_fail_null_user()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        await Assert.ThrowsAsync<Exception>(()=> _repository.UpdateRefreshTokenAsync(null));
    }
    
    [Fact]
    public async Task UpdateRefreshTokenAsync_fail_notFound_user()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        await Assert.ThrowsAsync<Exception>(()=> _repository.UpdateRefreshTokenAsync(new ExtendedIdentityUser
        {
            Id = 1,
            Email = "test",
        }));
    }


    [Fact]
    public async Task CanselRefreshToken_Success()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        var part = new ParticipantRegisterDTO
        {
            Email = "test",
            FirstName = "test",
            LastName = "test",
            Password = "test",
            BirthDate = new DateTime(),
            RegistrationDate = new DateTime()
        };
        var _repository = new ParticipantRepository(_context,_mapper);

        await _repository.RegisterParticipantAsync(part);
        await _repository.AddRefreshTokenField(part);
        
        var user = _context.Participants.FirstOrDefaultAsync(p => p.Email == "test").Result;
        await _repository.CanselRefreshToken(user.Id);
        
        var user1 = await _context.ExtendedIdentityUsers.FirstOrDefaultAsync(u => u.Email == user.Email);
        
        Assert.NotNull(user1);
        Assert.True(user1.RefreshTokenExpiryTime < DateTime.UtcNow);
        

    }
    
    [Fact]
    public async Task CanselRefreshToken_Fail_notFound()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);

        await Assert.ThrowsAsync<Exception>(() => _repository.CanselRefreshToken(-1));


    }
}