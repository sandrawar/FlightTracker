namespace FlightApp.Query
{
    internal static class QuerySyntax
    {
        public static class Airport
        {
            public static string[] Fields = [
                IdField, 
                AmslField,
                CodeField, 
                CountryCodeField, 
                NameField,
                WorldPositionField];

            public const string IdField = "ID";
            public const string AmslField = "AMSL";
            public const string CodeField = "Code";
            public const string CountryCodeField = "CountryCode";
            public const string NameField = "Name";
            public const string WorldPositionField = "WorldPosition";
        }

        public static class Cargo
        {
            public static string[] Fields = [
                IdField, 
                CodeField, 
                DescriptionField, 
                WeightField];

            public const string IdField = "ID";
            public const string CodeField = "Code";
            public const string DescriptionField = "Description";
            public const string WeightField = "Weight";
        }

        public static class CargoPlane
        {
            public static string[] Fields = [IdField, 
                CountryCodeField, 
                MaxLoadField, 
                ModelField, 
                SerialField];

            public const string IdField = "ID";
            public const string CountryCodeField = "CountryCode";
            public const string MaxLoadField = "MaxLoad";
            public const string ModelField = "Model";
            public const string SerialField = "Serial";
        }

        public static class Crew
        {
            public static string[] Fields = [
                IdField, 
                AgeField, 
                EmailField, 
                NameField, 
                PhoneField, 
                PractiseField, 
                RoleField];

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
            public static string[] Fields = [
                IdField, 
                AmslField, 
                LandingTimeField, 
                OriginField, 
                PlaneField, 
                TakeofTimeField, 
                TargetField,
                WorldPositionField];

            public const string IdField = "ID";
            public const string AmslField = "AMSL";
            public const string LandingTimeField = "LandingTime";
            public const string OriginField = "Origin";
            public const string PlaneField = "Plane";
            public const string TakeofTimeField = "TakeofTime";
            public const string TargetField = "Target";
            public const string WorldPositionField = "WorldPosition";
        }

        public static class Passenger
        {
            public static string[] Fields = [
                IdField, 
                AgeField, 
                ClassField, 
                EmailField, 
                NameField, 
                PhoneField, 
                MilesField];

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
            public static string[] Fields = [IdField, 
                BusinessClassSizeField, 
                CountryCodeField, 
                EconomyClassSizeField, 
                FirstClassSizeField, 
                ModelField, 
                SerialField];

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
