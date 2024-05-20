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

        public ulong ObjectID { get; }

        public string PhoneNumber { get; }

        public string EmailAddress { get; }
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

        public ulong ObjectID { get; }

        public float Longitude { get; }

        public float Latitude { get; }

        public float AMSL { get; }
    }

    internal sealed class IDUpdateData
    {
        public IDUpdateData(ulong objectID, ulong newObjectID)
        {
            ObjectID = objectID;
            NewObjectID = newObjectID;
        }

        public ulong ObjectID { get; }

        public ulong NewObjectID { get; }
    }


}
