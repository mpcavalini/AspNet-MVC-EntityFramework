using System.Data.Entity.ModelConfiguration;
using PetStore.Core.Domain;

namespace PetStore.Services.EntityConfigurations
{
    public class PetConfiguration : EntityTypeConfiguration<Pet>
    {
        public PetConfiguration()
        {
            Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(120);

            Property(s => s.DateOfBirth)
                .IsRequired()
                .HasColumnType("Date");

            Property(s => s.Weight)
                .IsRequired()
                .HasPrecision(4,2);

            Property(s => s.Type)
                .IsRequired();
                
            // Make sure that no two pets in the database have the same name and date of birth
            HasIndex(s => new { s.Name, s.DateOfBirth }).IsUnique();
        }
    }
}
