using System;
using System.Threading.Tasks;
using AutoMapper;
using EventManagement.Infrastructure.Persistence;
using EventManagement.Infrastructure.Repositories;

namespace EventManagement.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _contex;
        
        public UnitOfWork(ApplicationDbContext contex)
        {
            _contex = contex;
        }

        public async Task CompleteAsync()
        {
            await _contex.SaveChangesAsync();
        }
        
    }
}
