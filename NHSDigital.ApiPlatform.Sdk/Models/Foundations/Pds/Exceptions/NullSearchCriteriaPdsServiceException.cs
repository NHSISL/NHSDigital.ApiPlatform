// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.Exceptions
{
    public class NullSearchCriteriaPdsServiceException : Xeption
    {
        public NullSearchCriteriaPdsServiceException(string message)
            : base(message)
        { }
    }
}
