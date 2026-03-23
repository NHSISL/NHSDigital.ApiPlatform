// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.Exceptions
{
    public class InvalidArgumentPdsServiceException : Xeption
    {
        public InvalidArgumentPdsServiceException(string message)
            : base(message)
        { }
    }
}
