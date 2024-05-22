namespace FlightApp.Query
{
    internal static class QuerySyntax
    {
        public static class WorldPosition
        {
            public const string ClassName = "WorldPosition";

            public static string[] CoreFields = [
                LatitudeField,
                LongitudeField,
            ];
            public static string[] AllFields = [
                ..CoreFields,
            ];

            public const string LatitudeField = "Lat";
            public const string LongitudeField = "Long";
        }
        public static class Airport
        {
            public const string ClassName = "Airport";

            public static string[] CoreFields = [
                IdField,
                AmslField,
                CodeField,
                CountryCodeField,
                NameField,
                WorldPositionField,
            ];

            public static string[] AllFields = [
                ..CoreFields,
                ..WorldPosition.AllFields.Select(w => $"{WorldPositionField}.{w}")
            ];

            public const string IdField = "ID";
            public const string AmslField = "AMSL";
            public const string CodeField = "Code";
            public const string CountryCodeField = "CountryCode";
            public const string NameField = "Name";
            public const string WorldPositionField = "WorldPosition";
        }

        public static class Cargo
        {
            public const string ClassName = "Cargo";

            public static string[] CoreFields = [
                IdField,
                CodeField,
                DescriptionField,
                WeightField
            ];

            public static string[] AllFields = [
                ..CoreFields,
            ];

            public const string IdField = "ID";
            public const string CodeField = "Code";
            public const string DescriptionField = "Description";
            public const string WeightField = "Weight";
        }

        public static class CargoPlane
        {
            public const string ClassName = "CargoPlane";

            public static string[] CoreFields = [IdField,
                CountryCodeField,
                MaxLoadField,
                ModelField,
                SerialField
            ];

            public static string[] AllFields = [
                ..CoreFields,
            ];

            public const string IdField = "ID";
            public const string CountryCodeField = "CountryCode";
            public const string MaxLoadField = "MaxLoad";
            public const string ModelField = "Model";
            public const string SerialField = "Serial";
        }

        public static class Crew
        {
            public const string ClassName = "Crew";

            public static string[] CoreFields = [
                IdField,
                AgeField,
                EmailField,
                NameField,
                PhoneField,
                PractiseField,
                RoleField
            ];

            public static string[] AllFields = [
                ..CoreFields,
            ];

            public const string IdField = "ID";
            public const string AgeField = "Age";
            public const string EmailField = "Email";
            public const string NameField = "Name";
            public const string PhoneField = "Phone";
            public const string PractiseField = "Practise";
            public const string RoleField = "Role";
        }

        public static class Flight
        {
            public const string ClassName = "Flight";

            public static string[] CoreFields = [
                IdField,
                AmslField,
                LandingTimeField,
                OriginField,
                PlaneField,
                TakeoffTimeField,
                TargetField,
                WorldPositionField,
            ];

            public static string[] AllFields = [
                ..CoreFields,
                ..WorldPosition.AllFields.Select(w => $"{WorldPositionField}.{w}")
                ];

            public const string IdField = "ID";
            public const string AmslField = "AMSL";
            public const string LandingTimeField = "LandingTime";
            public const string OriginField = "Origin";
            public const string PlaneField = "Plane";
            public const string TakeoffTimeField = "TakeoffTime";
            public const string TargetField = "Target";
            public const string WorldPositionField = "WorldPosition";
        }

        public static class Passenger
        {
            public const string ClassName = "Passenger";

            public static string[] CoreFields = [
                IdField,
                AgeField,
                ClassField,
                EmailField,
                NameField,
                PhoneField,
                MilesField
            ];

            public static string[] AllFields = [
                ..CoreFields,
            ];

            public const string IdField = "ID";
            public const string AgeField = "Age";
            public const string EmailField = "Email";
            public const string ClassField = "Class";
            public const string MilesField = "Miles";
            public const string NameField = "Name";
            public const string PhoneField = "Phone";
        }

        public static class PassengerPlane
        {
            public const string ClassName = "PassengerPlane";

            public static string[] CoreFields = [IdField,
                BusinessClassSizeField,
                CountryCodeField,
                EconomyClassSizeField,
                FirstClassSizeField,
                ModelField,
                SerialField
            ];

            public static string[] AllFields = [
                ..CoreFields,
            ];

            public const string IdField = "ID";
            public const string BusinessClassSizeField = "BusinessClassSize";
            public const string CountryCodeField = "CountryCode";
            public const string EconomyClassSizeField = "EconomyClassSize";
            public const string FirstClassSizeField = "FirstClassSize";
            public const string ModelField = "Model";
            public const string SerialField = "Serial";
        }
    }
}
