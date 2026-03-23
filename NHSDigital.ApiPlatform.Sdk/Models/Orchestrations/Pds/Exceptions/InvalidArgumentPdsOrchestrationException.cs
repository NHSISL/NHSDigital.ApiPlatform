// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Models.Orchestrations.Pds.Exceptions
{
    public class InvalidArgumentPdsOrchestrationException : Xeption
    {
        public InvalidArgumentPdsOrchestrationException(string message)
            : base(message)
        { }
    }
}
