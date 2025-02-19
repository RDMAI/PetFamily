using PetFamily.Domain.Shared;

namespace PetFamily.Domain.Helpers;

public static class ErrorHelper
{
    public static class General
    {
        public static Error ValueIsInvalid(string? name = null)
        {
            var label = name ?? "value";
            return Error.Validation("value.is.invalid", $"{label} is invalid");
        }
        public static Error ValueIsNullOrEmpty(string? name = null)
        {
            var label = name ?? "value";
            return Error.Validation("value.is.null.or.empty", $"{label} is null or empty");
        }
        public static Error ValueIsNull(string? name = null)
        {
            var label = name ?? "value";
            return Error.Validation("value.is.null", $"{label} is null");
        }
        public static Error NotFound(Guid? id = null)
        {
            var label = id == null ? "" : $" for Id: {id}";
            return Error.Validation("record.not.found", $"record not found{label}");
        }
    }
}
