using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain.PetsManagement.Entities;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.SpeciesManagement.ValueObjects;
using PetFamily.Infrastructure.DataBaseAccess.Write.Converters;

namespace PetFamily.Infrastructure.DataBaseAccess.Write.Configurations;

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("pets");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasConversion(
                id => id.Value,
                value => PetId.Create(value));

        builder.ComplexProperty(d => d.Name, ib =>
        {
            ib.Property(name => name.Value)
                .IsRequired()
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH)
                .HasColumnName("name");
        });

        builder.ComplexProperty(d => d.Description, ib =>
        {
            ib.Property(description => description.Value)
                .IsRequired()
                .HasMaxLength(Constants.MAX_HIGH_TEXT_LENGTH)
                .HasColumnName("description");
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
                .IsRequired()
                .HasColumnName("weight");
        });

        builder.ComplexProperty(d => d.Height, ib =>
        {
            ib.Property(height => height.Value)
                .IsRequired()
                .HasColumnName("height");
        });

        builder.ComplexProperty(d => d.Breed, ib =>
        {
            ib.Property(breed => breed.BreedId)
                .HasConversion(
                    id => id.Value,
                    value => BreedId.Create(value))
                .IsRequired()
                .HasColumnName("breed_id");

            ib.Property(breed => breed.SpeciesId)
                .HasConversion(
                    id => id.Value,
                    value => SpeciesId.Create(value))
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

        builder.ComplexProperty(d => d.SerialNumber, ib =>
        {
            ib.Property(p => p.Value)
                .IsRequired()
                .HasColumnName("serial_number");
        });

        builder.Property(d => d.Photos)
            .HasConversion(
                photosToDB => ValueObjectListJSONConverter.Serialize(photosToDB),
                jsonFromDB => ValueObjectListJSONConverter.Deserialize<FileVO>(jsonFromDB),
                ValueObjectListJSONConverter.GetValueComparer<FileVO>())
            .HasColumnType("jsonb")
            .HasColumnName("photos");

        builder.Property(d => d.Requisites)
            .HasConversion(
                reqToDB => ValueObjectListJSONConverter.Serialize(reqToDB),
                jsonFromDB => ValueObjectListJSONConverter.Deserialize<Requisites>(jsonFromDB),
                ValueObjectListJSONConverter.GetValueComparer<Requisites>())
            .HasColumnType("jsonb")
            .HasColumnName("requisites");

        builder.Property(d => d.IsDeleted)
            .HasColumnName("is_deleted");

        builder.Property(d => d.DeletionDate)
            .IsRequired(false)
            .HasColumnName("deletion_date");

        builder.Property(d => d.CreationDate)
            .HasConversion(
                src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
                dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc)
            ).IsRequired();
    }
}
