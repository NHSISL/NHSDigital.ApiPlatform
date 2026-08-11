// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Models.Orchestrations.Pds.Exceptions
{
    public class TimeoutPdsOrchestrationException : Xeption
    {
        public TimeoutPdsOrchestrationException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
