// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Models.Clients.CareIdentityService.Exceptions
{
    public class CareIdentityServiceClientValidationException : Xeption
    {
        public CareIdentityServiceClientValidationException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
