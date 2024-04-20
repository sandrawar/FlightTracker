using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightApp.DataProcessor
{
    internal sealed class ContactInfoUpdateData
    {
        public ContactInfoUpdateData(ulong objectID, string phoneNumber, string emailAddress)
        {
            ObjectID = objectID;
            PhoneNumber = phoneNumber;
            EmailAddress = emailAddress;
        }

        public ulong ObjectID { get; init; }

        public string PhoneNumber { get; init; }

        public string EmailAddress { get; init; }
    }

    internal sealed class PositionUpdateData
    {
        public PositionUpdateData(ulong objectID, float longitude, float latitude, float aMSL)
        {
            ObjectID = objectID;
            Longitude = longitude;
            Latitude = latitude;
            AMSL = aMSL;
        }

        public ulong ObjectID { get; init; }

        public float Longitude { get; init; }

        public float Latitude { get; init; }

        public float AMSL { get; init; }
    }

    internal sealed class IDUpdateData
    {
        public IDUpdateData(ulong objectID, ulong newObjectID)
        {
            ObjectID = objectID;
            NewObjectID = newObjectID;
        }

        public ulong ObjectID { get; init; }

        public ulong NewObjectID { get; init; }
    }


}
