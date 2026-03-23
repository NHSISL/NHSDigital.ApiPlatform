// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.Exceptions
{
    public class PdsServiceValidationException : Xeption
    {
        public PdsServiceValidationException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
