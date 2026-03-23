// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Models.Processings.CareIdentityServices.Exceptions
{
    public class FailedCareIdentityServiceProcessingException : Xeption
    {
        public FailedCareIdentityServiceProcessingException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
