using PetFamily.Domain.PetsContext.ValueObjects.Volunteers;
using PetFamily.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Application.Volunteers.Filters;
public record VolunteerFilter(VolunteerFullName? FullName = null, Phone? Phone = null, Email? Email = null)
{
}
