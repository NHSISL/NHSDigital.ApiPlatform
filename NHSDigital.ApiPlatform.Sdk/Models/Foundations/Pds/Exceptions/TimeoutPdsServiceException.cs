// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.Exceptions
{
    public class TimeoutPdsServiceException : Xeption
    {
        public TimeoutPdsServiceException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
