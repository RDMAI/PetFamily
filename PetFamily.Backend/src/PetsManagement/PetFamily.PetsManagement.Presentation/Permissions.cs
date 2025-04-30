namespace PetFamily.PetsManagement.Presentation;

public static class Permissions
{
    public static class Pets
    {
        public const string CREATE = "pets.create";
        public const string READ = "pets.read";
        public const string UPDATE = "pets.update";
        public const string DELETE = "pets.delete";
    }
    public static class Volunteers
    {
        public const string CREATE = "volunteers.create";
        public const string READ = "volunteers.read";
        public const string UPDATE = "volunteers.update";
        public const string DELETE = "volunteers.delete";
    }
}
