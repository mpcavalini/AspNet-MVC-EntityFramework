using PetStore.Core.Domain;
using PetStore.Core.Repositories;

namespace PetStore.Services.Repositories
{
    public class PetRepository : Repository<Pet>, IPetRepository
    {
        public PetRepository(PetStoreDbContext context) : base(context)
        {
        }

        public PetStoreDbContext PetStoreDbContext
        {
            get { return Context as PetStoreDbContext; }
        }
    }
}
