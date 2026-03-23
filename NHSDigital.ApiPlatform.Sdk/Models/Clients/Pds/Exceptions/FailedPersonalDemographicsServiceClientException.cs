// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Models.Clients.Pds.Exceptions
{
    public class FailedPersonalDemographicsServiceClientException : Xeption
    {
        public FailedPersonalDemographicsServiceClientException(
            string message,
            Exception innerException,
            IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
