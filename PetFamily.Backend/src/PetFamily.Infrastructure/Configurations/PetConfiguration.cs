using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain.PetsContext.Entities;
using PetFamily.Domain.PetsContext.ValueObjects.Pets;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.SpeciesContext.ValueObjects;

namespace PetFamily.Infrastructure.Configurations;

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("Pets");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasConversion(
                id => id.Value,
                value => PetId.Create(value));

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);

        builder.Property(d => d.Description)
            .IsRequired()
            .HasMaxLength(Constants.MAX_HIGH_TEXT_LENGTH);

        builder.Property(d => d.Color)
            .HasConversion(
                d => d.Value,
                value => PetColor.Create(value).Value)
            .IsRequired()
            .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);

        builder.Property(d => d.Weight)
            .IsRequired();

        builder.Property(d => d.Height)
            .IsRequired();

        // Revisit this later !!!!!
        builder.OwnsOne(d => d.Breed, ib =>
        {
            ib.ToJson();

            ib.Property(d1 => d1.BreedId)
                .IsRequired()
                .HasColumnName("breed_id");

            ib.Property(d1 => d1.SpeciesId)
                .IsRequired()
                .HasColumnName("species_id");
        });

        builder.Property(d => d.HealthInformation)
            .HasConversion(
                d => d.Value,
                value => PetHealthInfo.Create(value).Value)
            .IsRequired();

        builder.OwnsOne(d => d.Address, ib =>
        {
            ib.ToJson();

            ib.Property(d1 => d1.City)
                .IsRequired()
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);

            ib.Property(d1 => d1.Street)
                .IsRequired()
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);

            ib.Property(d1 => d1.HouseNumber)
                .IsRequired();

            ib.Property(d1 => d1.HouseSubNumber)
                .IsRequired(false);

            ib.Property(d1 => d1.AppartmentNumber)
                .IsRequired(false);
        });

        builder.Property(d => d.OwnerPhone)
            .HasConversion(
                d => d.Value,
                value => Phone.Create(value).Value);

        builder.Property(d => d.IsCastrated)
            .IsRequired();

        builder.Property(d => d.BirthDate);

        builder.Property(d => d.IsVacinated)
            .IsRequired();

        builder.Property(d => d.Status)
            .HasConversion(
                d => d.Value,
                value => PetStatus.Create(value).Value)
            .IsRequired();

        builder.OwnsOne(d => d.Requisites, ib =>
        {
            ib.ToJson();

            ib.Property(d1 => d1.Name)
                .IsRequired()
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);

            ib.Property(d1 => d1.Value)
                .IsRequired()
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);

            ib.Property(d1 => d1.Description)
                .IsRequired()
                .HasMaxLength(Constants.MAX_HIGH_TEXT_LENGTH);
        });

        builder.Property(d => d.CreationDate)
            .IsRequired();
    }
}
