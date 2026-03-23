// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Models.Processings.CareIdentityServices.Exceptions
{
    public class CareIdentityServiceProcessingServiceException : Xeption
    {
        public CareIdentityServiceProcessingServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
