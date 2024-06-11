using System;
using System.Threading.Tasks;
using EventManagement.Infrastructure.Persistence;
using EventManagement.Infrastructure.Repositories;

namespace EventManagement.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private IEventRepository _eventRepository;
        private IParticipantRepository _participantRepository;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IEventRepository EventRepository
        {
            get
            {
                if (_eventRepository == null)
                {
                    _eventRepository = new EventRepository(_dbContext, this);
                }
                return _eventRepository;
            }
        }

        public IParticipantRepository ParticipantRepository
        {
            get
            {
                if (_participantRepository == null)
                {
                    _participantRepository = new ParticipantRepository(_dbContext, this);
                }
                return _participantRepository;
            }
        }

        public async Task<int> CompleteAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}