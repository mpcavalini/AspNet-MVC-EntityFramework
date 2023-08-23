using PetStore.Core.Repositories;

namespace PetStore.Core.UnitOfWork
{
    public interface IPetUnitOfWork : IUnitOfWork
    {
        IPetRepository Pets { get; }
    }
}
