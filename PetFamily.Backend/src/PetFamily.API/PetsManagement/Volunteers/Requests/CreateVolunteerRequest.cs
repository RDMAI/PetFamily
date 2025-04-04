﻿using PetFamily.Application.PetsManagement.Volunteers.Commands.CreateVolunteer;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.Shared.DTOs;

namespace PetFamily.API.PetsManagement.Volunteers.Requests;

public record CreateVolunteerRequest(
    string FirstName,
    string LastName,
    string FatherName,
    string Email,
    string Description,
    float ExperienceYears,
    string Phone,
    IEnumerable<RequisitesDTO> RequisitesList,
    IEnumerable<SocialNetworkDTO> SocialNetworksList)
{
    public CreateVolunteerCommand ToCommand()
    {
        return new CreateVolunteerCommand(
            FirstName,
            LastName,
            FatherName,
            Email,
            Description,
            ExperienceYears,
            Phone,
            RequisitesList,
            SocialNetworksList);
    }
}
