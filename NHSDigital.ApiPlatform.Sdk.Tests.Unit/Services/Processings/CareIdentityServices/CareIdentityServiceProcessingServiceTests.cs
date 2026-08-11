// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Processings.CareIdentityServices.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Services.Foundations.CareIdentityServices;
using NHSDigital.ApiPlatform.Sdk.Services.Processings.CareIdentityServices;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Processings.CareIdentityServices
{
    public partial class CareIdentityServiceProcessingServiceTests
    {
        private readonly Mock<ICareIdentityService> careIdentityServiceMock;
        private readonly ICareIdentityServiceProcessingService careIdentityServiceProcessingService;

        public CareIdentityServiceProcessingServiceTests()
        {
            this.careIdentityServiceMock = new Mock<ICareIdentityService>();

            this.careIdentityServiceProcessingService =
                new CareIdentityServiceProcessingService(this.careIdentityServiceMock.Object);
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            var innerException = new Xeption(message: GetRandomString());

            return new TheoryData<Xeption>
            {
                new CareIdentityServiceValidationException(GetRandomString(), innerException),
                new CareIdentityServiceDependencyValidationException(GetRandomString(), innerException)
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            var innerException = new Xeption(message: GetRandomString());

            return new TheoryData<Xeption>
            {
                new CareIdentityServiceDependencyException(GetRandomString(), innerException),
                new CareIdentityServiceServiceException(GetRandomString(), innerException)
            };
        }

        public static TheoryData<Exception> ServiceExceptions() =>
            new TheoryData<Exception>
            {
                new Exception(),
                new InvalidOperationException(),
                new NotSupportedException()
            };

        public static TheoryData<string> InvalidTexts() =>
            new TheoryData<string>
            {
                null,
                string.Empty,
                " "
            };

        private static NhsUserInfo CreateRandomNhsUserInfo() =>
            new NhsUserInfo
            {
                NhsIdUserUid = GetRandomString(),
                Name = GetRandomString(),
                Sub = GetRandomString(),
                NhsIdNrbacRoles = new List<NhsNrbacRole>()
            };

        private static string GetRandomString() =>
            new MnemonicString(wordCount: 1, wordMinLength: 8, wordMaxLength: 12).GetValue();

        private static CareIdentityServiceProcessingServiceException CreateExpectedServiceException(
            Exception serviceException)
        {
            var failedCareIdentityServiceProcessingException =
                new FailedCareIdentityServiceProcessingException(
                    message: "Failed care identity service processing error occurred, please contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            return new CareIdentityServiceProcessingServiceException(
                message: "Care identity service processing error occurred, please contact support.",
                innerException: failedCareIdentityServiceProcessingException);
        }
    }
}
