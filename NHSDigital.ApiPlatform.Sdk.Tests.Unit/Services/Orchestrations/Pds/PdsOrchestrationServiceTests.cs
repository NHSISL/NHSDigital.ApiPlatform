// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Orchestrations.Pds.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Services.Foundations.CareIdentityServices;
using NHSDigital.ApiPlatform.Sdk.Services.Foundations.Pds;
using NHSDigital.ApiPlatform.Sdk.Services.Orchestrations.Pds;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Orchestrations.Pds
{
    public partial class PdsOrchestrationServiceTests
    {
        private readonly Mock<ICareIdentityService> careIdentityServiceMock;
        private readonly Mock<IPdsService> pdsServiceMock;
        private readonly IPdsOrchestrationService pdsOrchestrationService;

        public PdsOrchestrationServiceTests()
        {
            this.careIdentityServiceMock = new Mock<ICareIdentityService>();
            this.pdsServiceMock = new Mock<IPdsService>();

            this.pdsOrchestrationService = new PdsOrchestrationService(
                careIdentityService: this.careIdentityServiceMock.Object,
                pdsService: this.pdsServiceMock.Object);
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            var innerException = new Xeption(message: GetRandomString());

            return new TheoryData<Xeption>
            {
                new CareIdentityServiceValidationException(GetRandomString(), innerException),
                new CareIdentityServiceDependencyValidationException(GetRandomString(), innerException),
                new PdsServiceValidationException(GetRandomString(), innerException),
                new PdsServiceDependencyValidationException(GetRandomString(), innerException)
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            var innerException = new Xeption(message: GetRandomString());

            return new TheoryData<Xeption>
            {
                new CareIdentityServiceDependencyException(GetRandomString(), innerException),
                new CareIdentityServiceServiceException(GetRandomString(), innerException),
                new PdsServiceDependencyException(GetRandomString(), innerException),
                new PdsServiceException(GetRandomString(), innerException)
            };
        }

        public static TheoryData<Exception> ServiceExceptions() =>
            new TheoryData<Exception>
            {
                new Exception(),
                new InvalidOperationException(),
                new NotSupportedException()
            };

        private static SearchCriteria CreateRandomSearchCriteria() =>
            new SearchCriteria
            {
                NhsNumber = GetRandomString()
            };

        private static string GetRandomString() =>
            new MnemonicString(wordCount: 1, wordMinLength: 8, wordMaxLength: 12).GetValue();

        private static PdsOrchestrationServiceException CreateExpectedServiceException(Exception serviceException)
        {
            var failedPdsOrchestrationException =
                new FailedPdsOrchestrationException(
                    message: "Failed PDS orchestration service error occurred, please contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            return new PdsOrchestrationServiceException(
                message: "PDS orchestration service error occurred, please contact support.",
                innerException: failedPdsOrchestrationException);
        }
    }
}
