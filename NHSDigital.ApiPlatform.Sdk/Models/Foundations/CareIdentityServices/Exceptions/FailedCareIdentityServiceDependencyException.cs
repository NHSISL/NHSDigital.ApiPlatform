// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices.Exceptions
{
    public class FailedCareIdentityServiceDependencyException : Xeption
    {
        public FailedCareIdentityServiceDependencyException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
