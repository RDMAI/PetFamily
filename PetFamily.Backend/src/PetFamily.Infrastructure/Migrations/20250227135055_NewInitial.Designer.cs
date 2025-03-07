﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PetFamily.Infrastructure;

#nullable disable

namespace PetFamily.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDBContext))]
    [Migration("20250227135055_NewInitial")]
    partial class NewInitial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PetFamily.Domain.PetsContext.Entities.Pet", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateOnly>("BirthDate")
                        .HasColumnType("date")
                        .HasColumnName("birth_date");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("creation_date");

                    b.Property<bool>("IsCastrated")
                        .HasColumnType("boolean")
                        .HasColumnName("is_castrated");

                    b.Property<bool>("IsVacinated")
                        .HasColumnType("boolean")
                        .HasColumnName("is_vacinated");

                    b.Property<Guid?>("volunteer_id")
                        .HasColumnType("uuid")
                        .HasColumnName("volunteer_id");

                    b.ComplexProperty<Dictionary<string, object>>("Address", "PetFamily.Domain.PetsContext.Entities.Pet.Address#Address", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<int?>("AppartmentNumber")
                                .HasColumnType("integer")
                                .HasColumnName("appartment_number");

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("character varying(100)")
                                .HasColumnName("city");

                            b1.Property<int>("HouseNumber")
                                .HasColumnType("integer")
                                .HasColumnName("house_number");

                            b1.Property<int?>("HouseSubNumber")
                                .HasColumnType("integer")
                                .HasColumnName("house_subnumber");

                            b1.Property<string>("Street")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("character varying(100)")
                                .HasColumnName("street");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Breed", "PetFamily.Domain.PetsContext.Entities.Pet.Breed#PetBreed", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<Guid>("BreedId")
                                .HasColumnType("uuid")
                                .HasColumnName("breed_id");

                            b1.Property<Guid>("SpeciesId")
                                .HasColumnType("uuid")
                                .HasColumnName("species_id");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Color", "PetFamily.Domain.PetsContext.Entities.Pet.Color#PetColor", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("character varying(100)")
                                .HasColumnName("color");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Description", "PetFamily.Domain.PetsContext.Entities.Pet.Description#Description", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(2000)
                                .HasColumnType("character varying(2000)")
                                .HasColumnName("description");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("HealthInformation", "PetFamily.Domain.PetsContext.Entities.Pet.HealthInformation#PetHealthInfo", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("character varying(100)")
                                .HasColumnName("health_information");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Height", "PetFamily.Domain.PetsContext.Entities.Pet.Height#PetHeight", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<float>("Value")
                                .HasColumnType("real")
                                .HasColumnName("height");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Name", "PetFamily.Domain.PetsContext.Entities.Pet.Name#PetName", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("character varying(100)")
                                .HasColumnName("name");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("OwnerPhone", "PetFamily.Domain.PetsContext.Entities.Pet.OwnerPhone#Phone", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(10)
                                .HasColumnType("character varying(10)")
                                .HasColumnName("owner_phone");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Status", "PetFamily.Domain.PetsContext.Entities.Pet.Status#PetStatus", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<int>("Value")
                                .HasColumnType("integer")
                                .HasColumnName("status");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Weight", "PetFamily.Domain.PetsContext.Entities.Pet.Weight#PetWeight", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<float>("Value")
                                .HasColumnType("real")
                                .HasColumnName("weight");
                        });

                    b.HasKey("Id")
                        .HasName("pk_pets");

                    b.HasIndex("volunteer_id")
                        .HasDatabaseName("ix_pets_volunteer_id");

                    b.ToTable("Pets", (string)null);
                });

            modelBuilder.Entity("PetFamily.Domain.PetsContext.Entities.Volunteer", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.ComplexProperty<Dictionary<string, object>>("Description", "PetFamily.Domain.PetsContext.Entities.Volunteer.Description#Description", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(2000)
                                .HasColumnType("character varying(2000)")
                                .HasColumnName("description");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Email", "PetFamily.Domain.PetsContext.Entities.Volunteer.Email#Email", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("character varying(100)")
                                .HasColumnName("email");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("ExperienceYears", "PetFamily.Domain.PetsContext.Entities.Volunteer.ExperienceYears#VolunteerExperienceYears", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<float>("Value")
                                .HasColumnType("real")
                                .HasColumnName("experience_years");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("FullName", "PetFamily.Domain.PetsContext.Entities.Volunteer.FullName#VolunteerFullName", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("FatherName")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("character varying(100)")
                                .HasColumnName("father_name");

                            b1.Property<string>("FirstName")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("character varying(100)")
                                .HasColumnName("first_name");

                            b1.Property<string>("LastName")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("character varying(100)")
                                .HasColumnName("last_name");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Phone", "PetFamily.Domain.PetsContext.Entities.Volunteer.Phone#Phone", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(10)
                                .HasColumnType("character varying(10)")
                                .HasColumnName("phone");
                        });

                    b.HasKey("Id")
                        .HasName("pk_volunteers");

                    b.ToTable("Volunteers", (string)null);
                });

            modelBuilder.Entity("PetFamily.Domain.SpeciesContext.Entities.Breed", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid?>("species_id")
                        .HasColumnType("uuid")
                        .HasColumnName("species_id");

                    b.ComplexProperty<Dictionary<string, object>>("Name", "PetFamily.Domain.SpeciesContext.Entities.Breed.Name#BreedName", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("character varying(100)")
                                .HasColumnName("name");
                        });

                    b.HasKey("Id")
                        .HasName("pk_breeds");

                    b.HasIndex("species_id")
                        .HasDatabaseName("ix_breeds_species_id");

                    b.ToTable("Breeds", (string)null);
                });

            modelBuilder.Entity("PetFamily.Domain.SpeciesContext.Entities.Species", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.ComplexProperty<Dictionary<string, object>>("Name", "PetFamily.Domain.SpeciesContext.Entities.Species.Name#SpeciesName", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("character varying(100)")
                                .HasColumnName("name");
                        });

                    b.HasKey("Id")
                        .HasName("pk_species");

                    b.ToTable("Species", (string)null);
                });

            modelBuilder.Entity("PetFamily.Domain.PetsContext.Entities.Pet", b =>
                {
                    b.HasOne("PetFamily.Domain.PetsContext.Entities.Volunteer", null)
                        .WithMany("Pets")
                        .HasForeignKey("volunteer_id")
                        .HasConstraintName("fk_pets_volunteers_volunteer_id");

                    b.OwnsOne("PetFamily.Domain.Shared.ValueObjects.RequisitesList", "Requisites", b1 =>
                        {
                            b1.Property<Guid>("PetId")
                                .HasColumnType("uuid");

                            b1.HasKey("PetId")
                                .HasName("pk_pets");

                            b1.ToTable("Pets");

                            b1.ToJson("Requisites");

                            b1.WithOwner()
                                .HasForeignKey("PetId")
                                .HasConstraintName("fk_pets_pets_pet_id");

                            b1.OwnsMany("PetFamily.Domain.Shared.ValueObjects.Requisites", "List", b2 =>
                                {
                                    b2.Property<Guid>("RequisitesListPetId")
                                        .HasColumnType("uuid");

                                    b2.Property<int>("__synthesizedOrdinal")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("integer");

                                    b2.Property<string>("Description")
                                        .IsRequired()
                                        .HasMaxLength(2000)
                                        .HasColumnType("character varying(2000)");

                                    b2.Property<string>("Name")
                                        .IsRequired()
                                        .HasMaxLength(100)
                                        .HasColumnType("character varying(100)");

                                    b2.Property<string>("Value")
                                        .IsRequired()
                                        .HasMaxLength(100)
                                        .HasColumnType("character varying(100)");

                                    b2.HasKey("RequisitesListPetId", "__synthesizedOrdinal")
                                        .HasName("pk_pets");

                                    b2.ToTable("Pets");

                                    b2.WithOwner()
                                        .HasForeignKey("RequisitesListPetId")
                                        .HasConstraintName("fk_pets_pets_requisites_list_pet_id");
                                });

                            b1.Navigation("List");
                        });

                    b.Navigation("Requisites")
                        .IsRequired();
                });

            modelBuilder.Entity("PetFamily.Domain.PetsContext.Entities.Volunteer", b =>
                {
                    b.OwnsOne("PetFamily.Domain.Shared.ValueObjects.RequisitesList", "Requisites", b1 =>
                        {
                            b1.Property<Guid>("VolunteerId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.HasKey("VolunteerId");

                            b1.ToTable("Volunteers");

                            b1.ToJson("Requisites");

                            b1.WithOwner()
                                .HasForeignKey("VolunteerId")
                                .HasConstraintName("fk_volunteers_volunteers_id");

                            b1.OwnsMany("PetFamily.Domain.Shared.ValueObjects.Requisites", "List", b2 =>
                                {
                                    b2.Property<Guid>("RequisitesListVolunteerId")
                                        .HasColumnType("uuid");

                                    b2.Property<int>("__synthesizedOrdinal")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("integer");

                                    b2.Property<string>("Description")
                                        .IsRequired()
                                        .HasMaxLength(2000)
                                        .HasColumnType("character varying(2000)");

                                    b2.Property<string>("Name")
                                        .IsRequired()
                                        .HasMaxLength(100)
                                        .HasColumnType("character varying(100)");

                                    b2.Property<string>("Value")
                                        .IsRequired()
                                        .HasMaxLength(100)
                                        .HasColumnType("character varying(100)");

                                    b2.HasKey("RequisitesListVolunteerId", "__synthesizedOrdinal")
                                        .HasName("pk_volunteers");

                                    b2.ToTable("Volunteers");

                                    b2.WithOwner()
                                        .HasForeignKey("RequisitesListVolunteerId")
                                        .HasConstraintName("fk_volunteers_volunteers_requisites_list_volunteer_id");
                                });

                            b1.Navigation("List");
                        });

                    b.OwnsOne("PetFamily.Domain.Shared.ValueObjects.SocialNetworkList", "SocialNetworks", b1 =>
                        {
                            b1.Property<Guid>("VolunteerId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.HasKey("VolunteerId");

                            b1.ToTable("Volunteers");

                            b1.ToJson("SocialNetworks");

                            b1.WithOwner()
                                .HasForeignKey("VolunteerId")
                                .HasConstraintName("fk_volunteers_volunteers_id");

                            b1.OwnsMany("PetFamily.Domain.Shared.ValueObjects.SocialNetwork", "List", b2 =>
                                {
                                    b2.Property<Guid>("SocialNetworkListVolunteerId")
                                        .HasColumnType("uuid");

                                    b2.Property<int>("__synthesizedOrdinal")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("integer");

                                    b2.Property<string>("Link")
                                        .IsRequired()
                                        .HasMaxLength(500)
                                        .HasColumnType("character varying(500)");

                                    b2.Property<string>("Name")
                                        .IsRequired()
                                        .HasMaxLength(100)
                                        .HasColumnType("character varying(100)");

                                    b2.HasKey("SocialNetworkListVolunteerId", "__synthesizedOrdinal")
                                        .HasName("pk_volunteers");

                                    b2.ToTable("Volunteers");

                                    b2.WithOwner()
                                        .HasForeignKey("SocialNetworkListVolunteerId")
                                        .HasConstraintName("fk_volunteers_volunteers_social_network_list_volunteer_id");
                                });

                            b1.Navigation("List");
                        });

                    b.Navigation("Requisites")
                        .IsRequired();

                    b.Navigation("SocialNetworks")
                        .IsRequired();
                });

            modelBuilder.Entity("PetFamily.Domain.SpeciesContext.Entities.Breed", b =>
                {
                    b.HasOne("PetFamily.Domain.SpeciesContext.Entities.Species", null)
                        .WithMany("Breeds")
                        .HasForeignKey("species_id")
                        .HasConstraintName("fk_breeds_species_species_id");
                });

            modelBuilder.Entity("PetFamily.Domain.PetsContext.Entities.Volunteer", b =>
                {
                    b.Navigation("Pets");
                });

            modelBuilder.Entity("PetFamily.Domain.SpeciesContext.Entities.Species", b =>
                {
                    b.Navigation("Breeds");
                });
#pragma warning restore 612, 618
        }
    }
}
