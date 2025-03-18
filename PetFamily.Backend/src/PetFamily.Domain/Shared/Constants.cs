using System.Reflection;

namespace PetFamily.Domain.Shared
{
    public static class Constants
    {
        public static class BucketNames
        {
            public const string PET_PHOTOS = "petphotos";
            public const string PET_VIDEOS = "petvideos";
        }

        public const int MAX_LOW_TEXT_LENGTH = 100;
        public const int MAX_MID_TEXT_LENGTH = 500;
        public const int MAX_HIGH_TEXT_LENGTH = 2000;
    }
}
