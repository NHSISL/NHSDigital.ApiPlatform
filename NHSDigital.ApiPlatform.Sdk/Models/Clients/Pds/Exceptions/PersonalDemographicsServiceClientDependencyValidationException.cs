// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Models.Clients.Pds.Exceptions
{
    public class PersonalDemographicsServiceClientDependencyValidationException : Xeption
    {
        public PersonalDemographicsServiceClientDependencyValidationException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
