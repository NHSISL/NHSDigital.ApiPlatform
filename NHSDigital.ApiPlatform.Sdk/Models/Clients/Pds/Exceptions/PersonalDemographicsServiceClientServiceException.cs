// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Models.Clients.Pds.Exceptions
{
    public class PersonalDemographicsServiceClientServiceException : Xeption
    {
        public PersonalDemographicsServiceClientServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
