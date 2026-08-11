// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices.Exceptions
{
    public class InvalidCareIdentityServiceDependencyException : Xeption
    {
        public InvalidCareIdentityServiceDependencyException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
