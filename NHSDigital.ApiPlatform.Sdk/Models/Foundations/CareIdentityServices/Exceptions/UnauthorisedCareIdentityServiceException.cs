// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices.Exceptions
{
    public class UnauthorisedCareIdentityServiceException : Xeption
    {
        public UnauthorisedCareIdentityServiceException(string message)
            : base(message)
        { }
    }
}
