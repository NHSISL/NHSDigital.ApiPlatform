// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Models.Clients.CareIdentityService.Exceptions
{
    public class TimeoutCareIdentityServiceClientException : Xeption
    {
        public TimeoutCareIdentityServiceClientException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
