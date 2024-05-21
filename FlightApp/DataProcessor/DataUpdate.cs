namespace FlightApp.DataProcessor
{
    internal record class ContactInfoUpdateData(ulong ObjectID, string PhoneNumber, string EmailAddress);

    internal sealed record PositionUpdateData(ulong ObjectID, float Longitude, float Latitude, float AMSL);

    internal sealed record IDUpdateData(ulong ObjectID, ulong NewObjectID);


}
