// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Models.Clients.Pds.Exceptions
{
    public class TimeoutPersonalDemographicsServiceClientException : Xeption
    {
        public TimeoutPersonalDemographicsServiceClientException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
