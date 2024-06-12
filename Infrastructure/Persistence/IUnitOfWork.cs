using System;
using System.Threading.Tasks;
using EventManagement.Infrastructure.Repositories;

namespace EventManagement.Infrastructure
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}