namespace FlightApp.Query.Validation
{
    internal abstract class FieldsValidationBuilder
    {
        protected FieldsValidationBuilder()
        {
            Validation = new FieldsValidation();
        }

        protected FieldsValidation Validation {get; }

        public virtual void AddOwnFields()
        {
        }

        public virtual void AddDependencyFields()
        {
        }

        public IFieldsValidation Build() => Validation;
    }

    internal class AirportFieldsValidationBuilder: FieldsValidationBuilder
    {
        public override void AddOwnFields()
        {
            Validation.AddValidFields(QuerySyntax.Airport.AllFields);
        }
    }

    internal class CargoFieldsValidationBuilder: FieldsValidationBuilder
    {
        public override void AddOwnFields()
        {
            Validation.AddValidFields(QuerySyntax.Cargo.AllFields);
        }
    }

    internal class CargoPlaneFieldsValidationBuilder: FieldsValidationBuilder
    {
        public override void AddOwnFields()
        {
            Validation.AddValidFields(QuerySyntax.CargoPlane.AllFields);
        }
    }

    internal class CrewFieldsValidationBuilder: FieldsValidationBuilder
    {
        public override void AddOwnFields()
        {
            Validation.AddValidFields(QuerySyntax.Crew.AllFields);
        }
    }

    internal class FlightFieldsValidationBuilder: FieldsValidationBuilder
    {
        public override void AddOwnFields()
        {
            Validation.AddValidFields(QuerySyntax.Flight.AllFields);
        }
    }

    internal class PassengerFieldsValidationBuilder: FieldsValidationBuilder
    {
        public override void AddOwnFields()
        {
            Validation.AddValidFields(QuerySyntax.Passenger.AllFields);
        }
    }

    internal class PassengerPlaneFieldsValidationBuilder: FieldsValidationBuilder
    {
        public override void AddOwnFields()
        {
            Validation.AddValidFields(QuerySyntax.PassengerPlane.AllFields);
        }
    }
}
