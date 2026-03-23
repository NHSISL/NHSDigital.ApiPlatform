// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Models.Processings.CareIdentityServices.Exceptions
{
    public class UnauthorisedCareIdentityServiceProcessingException : Xeption
    {
        public UnauthorisedCareIdentityServiceProcessingException(string message)
            : base(message)
        { }
    }
}
