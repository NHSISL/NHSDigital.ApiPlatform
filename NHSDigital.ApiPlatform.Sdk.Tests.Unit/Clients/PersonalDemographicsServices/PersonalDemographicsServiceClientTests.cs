// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Clients.PersonalDemographicsServices;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using NHSDigital.ApiPlatform.Sdk.Models.Orchestrations.Pds.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Services.Orchestrations.Pds;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Clients.PersonalDemographicsServices
{
    public partial class PersonalDemographicsServiceClientTests
    {
        private readonly Mock<IPdsOrchestrationService> pdsOrchestrationServiceMock;
        private readonly IPersonalDemographicsServiceClient personalDemographicsServiceClient;

        public PersonalDemographicsServiceClientTests()
        {
            this.pdsOrchestrationServiceMock = new Mock<IPdsOrchestrationService>();

            this.personalDemographicsServiceClient =
                new PersonalDemographicsServiceClient(this.pdsOrchestrationServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var innerException = new Xeption(message: GetRandomString());

            return new TheoryData<Xeption>
            {
                new PdsOrchestrationValidationException(GetRandomString(), innerException)
            };
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            var innerException = new Xeption(message: GetRandomString());

            return new TheoryData<Xeption>
            {
                new PdsOrchestrationDependencyValidationException(GetRandomString(), innerException)
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            var innerException = new Xeption(message: GetRandomString());

            return new TheoryData<Xeption>
            {
                new PdsOrchestrationDependencyException(GetRandomString(), innerException)
            };
        }

        public static TheoryData<Xeption> ServiceExceptions()
        {
            var innerException = new Xeption(message: GetRandomString());

            return new TheoryData<Xeption>
            {
                new PdsOrchestrationServiceException(GetRandomString(), innerException)
            };
        }

        public static TheoryData<Exception> UnexpectedExceptions() =>
            new TheoryData<Exception>
            {
                new Exception(),
                new InvalidOperationException(),
                new TimeoutException()
            };

        private static SearchCriteria CreateRandomSearchCriteria() =>
            new SearchCriteria
            {
                NhsNumber = new IntRange(min: 1000000000, max: 1999999999).GetValue().ToString()
            };

        private static string GetRandomString() =>
            new MnemonicString(wordCount: 1, wordMinLength: 8, wordMaxLength: 12).GetValue();
    }
}
