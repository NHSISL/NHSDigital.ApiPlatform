// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Models.Orchestrations.Pds.Exceptions
{
    public class NullSearchCriteriaPdsOrchestrationException : Xeption
    {
        public NullSearchCriteriaPdsOrchestrationException(string message)
            : base(message)
        { }
    }
}
