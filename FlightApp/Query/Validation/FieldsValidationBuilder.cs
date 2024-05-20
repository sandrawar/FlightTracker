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
            Validation.AddValidFields(QuerySyntax.Airport.Fields);
        }
    }

    internal class CargoFieldsValidationBuilder: FieldsValidationBuilder
    {
        public override void AddOwnFields()
        {
            Validation.AddValidFields(QuerySyntax.Cargo.Fields);
        }
    }

    internal class CargoPlaneFieldsValidationBuilder: FieldsValidationBuilder
    {
        public override void AddOwnFields()
        {
            Validation.AddValidFields(QuerySyntax.CargoPlane.Fields);
        }
    }

    internal class CrewFieldsValidationBuilder: FieldsValidationBuilder
    {
        public override void AddOwnFields()
        {
            Validation.AddValidFields(QuerySyntax.Crew.Fields);
        }
    }

    internal class FlightFieldsValidationBuilder: FieldsValidationBuilder
    {
        public override void AddOwnFields()
        {
            Validation.AddValidFields(QuerySyntax.Flight.Fields);
        }
    }

    internal class PassengerFieldsValidationBuilder: FieldsValidationBuilder
    {
        public override void AddOwnFields()
        {
            Validation.AddValidFields(QuerySyntax.Passenger.Fields);
        }
    }

    internal class PassengerPlaneFieldsValidationBuilder: FieldsValidationBuilder
    {
        public override void AddOwnFields()
        {
            Validation.AddValidFields(QuerySyntax.PassengerPlane.Fields);
        }
    }
}
