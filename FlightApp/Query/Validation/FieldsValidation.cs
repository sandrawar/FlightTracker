namespace FlightApp.Query.Validation
{
    internal interface IFieldsValidation
    {
        bool ValidateFields(IEnumerable<string> fields);
    }

    internal class FieldsValidation : IFieldsValidation
    {
        private ISet<string> validFields;

        public FieldsValidation()
        {
            validFields = new HashSet<string>(["*"]);
        }

        public bool ValidateFields(IEnumerable<string> fields)
            => fields.All(x => validFields.Contains(x));

        public void AddValidFields(IEnumerable<string> fields)
        {
            foreach (var field in fields)
            {
                validFields.Add(field);
            }
        }
    }
}
