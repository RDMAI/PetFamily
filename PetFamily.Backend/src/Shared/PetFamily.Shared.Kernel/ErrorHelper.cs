using System.Xml.Linq;

namespace PetFamily.Shared.Kernel;

public static class ErrorHelper
{
    public static class General
    {
        public static Error MethodNotApplicable(string? mes = null)
        {
            return Error.Failure(
                "method.not.applicable",
                mes ?? "Called method is not applicable");
        }

        public static Error ValueIsInvalid(string? name = null)
        {
            var label = name ?? "value";
            return Error.Validation("value.is.invalid", $"{label} is invalid", name);
        }
        public static Error ValueIsNullOrEmpty(string? name = null)
        {
            var label = name ?? "value";
            return Error.Validation("value.is.null.or.empty", $"{label} is null or empty", name);
        }
        public static Error ValueIsNull(string? name = null)
        {
            var label = name ?? "value";
            return Error.Validation("value.is.null", $"{label} is null", name);
        }
        public static Error NotFound(Guid? id = null)
        {
            var label = id == null ? "" : $" for Id: {id}";
            return Error.Validation("record.not.found", $"record not found{label}");
        }
        public static Error AlreadyExist(string? name = null)
        {
            var label = name ?? "value";
            return Error.Validation("record.exists", $"{label} exists");
        }
    }

    public static class Files
    {
        public static Error DeleteFailure(string? message = null)
        {
            return Error.Validation(
                "file.delete.failure",
                message ?? $"Failed to delete file from filestorage");
        }
        public static Error UploadFailure(string? message = null)
        {
            return Error.Validation(
                "file.upload.failure",
                message ?? $"Failed to upload file to filestorage");
        }
        public static Error GetFailure(string? message = null)
        {
            return Error.Validation(
                "file.get.failure",
                message ?? $"Failed to get file to filestorage");
        }
    }

    public static class Authentication
    {
        public static Error InvalidLoginAttempt()
        {
            return Error.Validation(
                "invalid.login.attempt",
                "Invalid login attempt");
        }

        public static Error UserNotFound(string? userName = null)
        {
            var message = userName is null ? "User not found" : $"User with username {userName} not found";

            return Error.Validation(
                "user.not.found",
                message);
        }

        public static Error UserAlreadyExist(string? userName = null)
        {
            var message = userName is null ? "User with this username already exist" : $"User with username {userName} already exist";

            return Error.Validation(
                "user.exists",
                message);
        }
    }
}
