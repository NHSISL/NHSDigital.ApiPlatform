// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Clients.CareIdentityServices;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices;
using NHSDigital.ApiPlatform.Sdk.Models.Processings.CareIdentityServices.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Services.Processings.CareIdentityServices;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Clients.CareIdentityServices
{
    public partial class CareIdentityServiceClientTests
    {
        private readonly Mock<ICareIdentityServiceProcessingService> careIdentityServiceProcessingServiceMock;
        private readonly ICareIdentityServiceClient careIdentityServiceClient;

        public CareIdentityServiceClientTests()
        {
            this.careIdentityServiceProcessingServiceMock =
                new Mock<ICareIdentityServiceProcessingService>();

            this.careIdentityServiceClient =
                new CareIdentityServiceClient(this.careIdentityServiceProcessingServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var innerException = new Xeption(message: GetRandomString());

            return new TheoryData<Xeption>
            {
                new CareIdentityServiceProcessingValidationException(GetRandomString(), innerException)
            };
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            var innerException = new Xeption(message: GetRandomString());

            return new TheoryData<Xeption>
            {
                new CareIdentityServiceProcessingDependencyValidationException(GetRandomString(), innerException)
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            var innerException = new Xeption(message: GetRandomString());

            return new TheoryData<Xeption>
            {
                new CareIdentityServiceProcessingDependencyException(GetRandomString(), innerException)
            };
        }

        public static TheoryData<Xeption> ServiceExceptions()
        {
            var innerException = new Xeption(message: GetRandomString());

            return new TheoryData<Xeption>
            {
                new CareIdentityServiceProcessingServiceException(GetRandomString(), innerException)
            };
        }

        public static TheoryData<Exception> UnexpectedExceptions() =>
            new TheoryData<Exception>
            {
                new Exception(),
                new InvalidOperationException(),
                new TimeoutException()
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
    }
}
