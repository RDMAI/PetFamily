namespace PetFamily.Domain.Shared;

public record Error
{
    public const string SEPARATOR = "||";

    public static Error Validation(string code, string message, string? invalidField = null) =>
        new(code, message, ErrorType.Validation, invalidField);
    public static Error NotFound(string code, string message) =>
        new(code, message, ErrorType.NotFound);
    public static Error Failure(string code, string message) =>
        new(code, message, ErrorType.Failure);
    public static Error Conflict(string code, string message) =>
        new(code, message, ErrorType.Conflict);

    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }
    public string? InvalidField { get; }

    public string Serialize()
    {
        return $"{Code}{SEPARATOR}{Message}{SEPARATOR}{Type}";
    }
    public static Error Deserialize(string serialized)
    {
        var strings = serialized.Split(SEPARATOR);

        if (strings.Length < 3)
            throw new ArgumentException("Invalid serialized error format");

        if (Enum.TryParse<ErrorType>(strings[2], out var type) == false)
            throw new ArgumentException("Invalid serialized error format");

        return new Error(strings[0], strings[1], type);
    }

    public ErrorList ToErrorList() => new ErrorList([this]);

    private Error(string code, string message, ErrorType type, string? invalidField = null)
    {
        Code = code;
        Message = message;
        Type = type;
        InvalidField = invalidField;
    }
}

public enum ErrorType
{
    Validation,
    NotFound,
    Failure,
    Conflict
}
