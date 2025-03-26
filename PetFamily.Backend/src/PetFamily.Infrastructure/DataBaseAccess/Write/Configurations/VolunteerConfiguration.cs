using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain.PetsManagement.Entities;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Infrastructure.DataBaseAccess.Write.Converters;

namespace PetFamily.Infrastructure.DataBaseAccess.Write.Configurations;

public class VolunteerConfiguration : IEntityTypeConfiguration<Volunteer>
{
    public void Configure(EntityTypeBuilder<Volunteer> builder)
    {
        builder.ToTable("volunteers");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasConversion(
                id => id.Value,
                value => VolunteerId.Create(value));

        builder.ComplexProperty(d => d.Description, ib =>
        {
            ib.Property(description => description.Value)
                .IsRequired()
                .HasMaxLength(Constants.MAX_HIGH_TEXT_LENGTH)
                .HasColumnName("description");
        });

        builder.ComplexProperty(d => d.FullName, ib =>
        {
            ib.Property(d1 => d1.FirstName)
                .IsRequired()
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH)
                .HasColumnName("first_name");

            ib.Property(d1 => d1.LastName)
                .IsRequired()
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH)
                .HasColumnName("last_name");

            ib.Property(d1 => d1.FatherName)
                .IsRequired()
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH)
                .HasColumnName("father_name");
        });

        builder.ComplexProperty(d => d.Email, ib =>
        {
            ib.Property(email => email.Value)
                .IsRequired()
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH)
                .HasColumnName("email");
        });

        builder.ComplexProperty(d => d.ExperienceYears, ib =>
        {
            ib.Property(experienceYears => experienceYears.Value)
                .IsRequired()
                .HasColumnName("experience_years");
        });

        builder.ComplexProperty(d => d.Phone, ib =>
        {
            ib.Property(phone => phone.Value)
                .IsRequired()
                .HasMaxLength(Phone.MAX_LENGTH)
                .HasColumnName("phone");
        });

        builder.Property(d => d.Requisites)
            .HasConversion(
                reqToDB => ValueObjectListJSONConverter.Serialize(reqToDB),
                jsonFromDB => ValueObjectListJSONConverter.Deserialize<Requisites>(jsonFromDB),
                ValueObjectListJSONConverter.GetValueComparer<Requisites>())
            .HasColumnType("jsonb")
            .HasColumnName("requisites");

        builder.Property(d => d.SocialNetworks)
            .HasConversion(
                reqToDB => ValueObjectListJSONConverter.Serialize(reqToDB),
                jsonFromDB => ValueObjectListJSONConverter.Deserialize<SocialNetwork>(jsonFromDB),
                ValueObjectListJSONConverter.GetValueComparer<SocialNetwork>())
            .HasColumnType("jsonb")
            .HasColumnName("social_networks");

        builder.HasMany(d => d.Pets)
            .WithOne()
            .HasForeignKey("volunteer_id")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Property(d => d.IsDeleted)
            .HasColumnName("is_deleted");

        builder.Property(d => d.DeletionDate)
            .IsRequired(false)
            .HasColumnName("deletion_date");

        builder.Navigation(d => d.Pets).AutoInclude();
    }
}
