// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices.Exceptions
{
    public class InvalidStateCareIdentityServiceException : Xeption
    {
        public InvalidStateCareIdentityServiceException(string message)
            : base(message)
        { }
    }
}
