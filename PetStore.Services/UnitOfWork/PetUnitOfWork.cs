using System;
using PetStore.Core.Repositories;
using PetStore.Core.UnitOfWork;
using PetStore.Services.Repositories;

namespace PetStore.Services.UnitOfWork
{
    public class PetUnitOfWork : IPetUnitOfWork
    {
        private readonly PetStoreDbContext _context;

        public IPetRepository Pets { get; private set; }

        public PetUnitOfWork()
        {
                
        }

        public PetUnitOfWork(PetStoreDbContext context)
        {
            _context = context;
            Pets = new PetRepository(_context);
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
