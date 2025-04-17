using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using PetFamily.SpeciesManagement.Domain.Entities;

namespace PetFamily.SpeciesManagement.Infrastructure.Database.Write.Configurations;

public class BreedConfiguration : IEntityTypeConfiguration<Breed>
{
    public void Configure(EntityTypeBuilder<Breed> builder)
    {
        builder.ToTable("breeds");

        builder.HasKey(d => d.Id);

        builder.ComplexProperty(d => d.Name, ib =>
        {
            ib.Property(name => name.Value)
                .IsRequired()
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH)
                .HasColumnName("name");
        });

        builder.Property(d => d.Id)
            .HasConversion(
                id => id.Value,
                value => BreedId.Create(value));
    }
}
