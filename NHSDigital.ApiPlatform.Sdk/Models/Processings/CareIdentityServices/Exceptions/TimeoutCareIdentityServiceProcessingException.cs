// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Models.Processings.CareIdentityServices.Exceptions
{
    public class TimeoutCareIdentityServiceProcessingException : Xeption
    {
        public TimeoutCareIdentityServiceProcessingException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
