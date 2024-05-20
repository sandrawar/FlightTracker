namespace FlightApp.Query.Validation
{
    internal static class FieldsValidationDirector
    {
        public static IFieldsValidation Prepare(FieldsValidationBuilder builder)
        {
            builder.AddOwnFields();
            builder.AddDependencyFields();

            return builder.Build();
        }
    }
}
