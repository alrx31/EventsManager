using System;
using System.Threading.Tasks;
using EventManagement.Infrastructure.Repositories;

namespace EventManagement.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IEventRepository EventRepository { get; }
        IParticipantRepository ParticipantRepository { get; }
        
        Task<int> CompleteAsync();
    }
}