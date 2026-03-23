// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Models.Processings.CareIdentityServices.Exceptions
{
    public class InvalidArgumentCareIdentityServiceProcessingException : Xeption
    {
        public InvalidArgumentCareIdentityServiceProcessingException(string message)
            : base(message)
        { }
    }
}
