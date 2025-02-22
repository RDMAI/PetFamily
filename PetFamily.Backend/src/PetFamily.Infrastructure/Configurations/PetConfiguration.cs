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

        builder.ComplexProperty(d => d.Name, ib =>
        {
            ib.Property(name => name.Value)
                .IsRequired()
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);
        });

        builder.ComplexProperty(d => d.Description, ib =>
        {
            ib.Property(description => description.Value)
                .IsRequired()
                .HasMaxLength(Constants.MAX_HIGH_TEXT_LENGTH);
        });
            
        builder.ComplexProperty(d => d.Color, ib =>
        {
            ib.Property(color => color.Value)
                .IsRequired()
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH)
                .HasColumnName("color");
        });

        builder.ComplexProperty(d => d.Weight, ib =>
        {
            ib.Property(weight => weight.Value)
                .IsRequired();
        });

        builder.ComplexProperty(d => d.Height, ib =>
        {
            ib.Property(height => height.Value)
                .IsRequired();
        });

        builder.ComplexProperty(d => d.Breed, ib =>
        {
            ib.Property(breed => breed.BreedId)
                .IsRequired()
                .HasColumnName("breed_id");

            ib.Property(breed => breed.SpeciesId)
                .IsRequired()
                .HasColumnName("species_id");
        });

        builder.ComplexProperty(d => d.HealthInformation, ib =>
        {
            ib.Property(health => health.Value)
                .IsRequired()
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH)
                .HasColumnName("health_information");
        });

        builder.ComplexProperty(d => d.Address, ib =>
        {
            ib.Property(adress => adress.City)
                .IsRequired()
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH)
                .HasColumnName("city");

            ib.Property(adress => adress.Street)
                .IsRequired()
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH)
                .HasColumnName("street");

            ib.Property(adress => adress.HouseNumber)
                .IsRequired()
                .HasColumnName("house_number");

            ib.Property(adress => adress.HouseSubNumber)
                .IsRequired(false)
                .HasColumnName("house_subnumber");

            ib.Property(adress => adress.AppartmentNumber)
                .IsRequired(false)
                .HasColumnName("appartment_number");
        });

        builder.ComplexProperty(d => d.OwnerPhone, ib =>
        {
            ib.Property(phone => phone.Value)
                .IsRequired()
                .HasMaxLength(Phone.MAX_LENGTH)
                .HasColumnName("owner_phone");
        });

        builder.Property(d => d.IsCastrated)
            .IsRequired();

        builder.Property(d => d.BirthDate);

        builder.Property(d => d.IsVacinated)
            .IsRequired();

        builder.ComplexProperty(d => d.Status, ib =>
        {
            ib.Property(p => p.Value)
                .IsRequired()
                .HasColumnName("status");
        });

        builder.OwnsOne(d => d.Requisites, ib =>
        {
            ib.ToJson();

            ib.OwnsMany(r => r.List, rb =>
            {
                rb.Property(r1 => r1.Name)
                    .IsRequired()
                    .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);

                rb.Property(r1 => r1.Description)
                    .IsRequired()
                    .HasMaxLength(Constants.MAX_HIGH_TEXT_LENGTH);

                rb.Property(r1 => r1.Value)
                    .IsRequired()
                    .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);
            });
        });

        builder.Property(d => d.CreationDate)
            .IsRequired();
    }
}
