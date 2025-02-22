﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain.PetsContext.Entities;
using PetFamily.Domain.PetsContext.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Infrastructure.Configurations;

public class VolunteerConfiguration : IEntityTypeConfiguration<Volunteer>
{
    public void Configure(EntityTypeBuilder<Volunteer> builder)
    {
        builder.ToTable("Volunteers");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasConversion(
                id => id.Value,
                value => VolunteerId.Create(value));

        builder.Property(d => d.Description)
            .IsRequired()
            .HasMaxLength(Constants.MAX_HIGH_TEXT_LENGTH);

        builder.OwnsOne(d => d.FullName, ib =>
        {
            ib.ToJson();

            ib.Property(d1 => d1.FirstName)
                .IsRequired()
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);

            ib.Property(d1 => d1.LastName)
                .IsRequired()
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);

            ib.Property(d1 => d1.FatherName)
                .IsRequired()
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);
        });

        builder.Property(d => d.Email)
            .HasConversion(
                d => d.Value,
                value => Email.Create(value).Value);

        builder.Property(d => d.ExperienceYears)
            .IsRequired();

        builder.Property(d => d.Phone)
            .HasConversion(
                d => d.Value,
                value => Phone.Create(value).Value);

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

        builder.OwnsOne(d => d.SocialNetworks, ib =>
        {
            ib.ToJson();

            ib.OwnsMany(s => s.List, sb =>
            {
                sb.Property(s1 => s1.Name)
                    .IsRequired()
                    .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);

                sb.Property(s1 => s1.Link)
                    .IsRequired()
                    .HasMaxLength(Constants.MAX_MID_TEXT_LENGTH);
            });
        });

        builder.HasMany(d => d.Pets)
            .WithOne()
            .HasForeignKey("volunteer_id");
    }
}
