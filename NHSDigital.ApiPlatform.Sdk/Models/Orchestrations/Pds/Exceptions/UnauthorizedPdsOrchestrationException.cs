// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Models.Orchestrations.Pds.Exceptions
{
    public class UnauthorizedPdsOrchestrationException : Xeption
    {
        public UnauthorizedPdsOrchestrationException(string message)
            : base(message)
        { }
    }
}
