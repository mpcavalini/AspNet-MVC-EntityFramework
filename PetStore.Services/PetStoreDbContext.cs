using System.Data.Entity;
using PetStore.Core.Domain;
using PetStore.Services.EntityConfigurations;

//using System.Diagnostics;

namespace PetStore.Services
{
    public class PetStoreDbContext : DbContext
    {
        public PetStoreDbContext()
            : base("name=PetStoreDbContext")
        {
            this.Configuration.LazyLoadingEnabled = false;

            // EF debug
            //Database.Log = s => Debug.WriteLine(s);
        }

        public virtual DbSet<Pet> Pets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new PetConfiguration());
        }
    }
}
