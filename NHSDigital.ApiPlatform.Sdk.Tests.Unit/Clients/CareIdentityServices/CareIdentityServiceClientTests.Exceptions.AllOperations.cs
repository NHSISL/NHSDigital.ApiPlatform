// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Clients.CareIdentityService.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Processings.CareIdentityServices.Exceptions;
using Xeptions;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Clients.CareIdentityServices
{
    public partial class CareIdentityServiceClientTests
    {
        // The client repeats the same catch ladder in all four of its methods, so each category is
        // exercised against every method rather than against whichever one happened to be picked.
        public static TheoryData<string> ClientOperations() =>
            new TheoryData<string>
            {
                nameof(ICareIdentityServiceClientOperation.BuildLoginUrl),
                nameof(ICareIdentityServiceClientOperation.Logout),
                nameof(ICareIdentityServiceClientOperation.GetAccessToken),
                nameof(ICareIdentityServiceClientOperation.GetUserInfo)
            };

        [Theory]
        [MemberData(nameof(ClientOperations))]
        public async Task ShouldThrowClientValidationExceptionOnEveryOperationIfValidationErrorOccursAsync(
            string operation)
        {
            // given
            var innerException = new Xeption(message: GetRandomString());

            var processingValidationException =
                new CareIdentityServiceProcessingValidationException(GetRandomString(), innerException);

            SetupEveryOperationToThrow(processingValidationException);

            // when
            Exception actualException = await InvokeOperationAsync(operation);

            // then
            actualException.Should().BeOfType<CareIdentityServiceClientValidationException>();
            actualException.InnerException.Should().BeSameAs(innerException);
        }

        [Theory]
        [MemberData(nameof(ClientOperations))]
        public async Task ShouldThrowClientDependencyValidationExceptionOnEveryOperationAsync(string operation)
        {
            // given
            var innerException = new Xeption(message: GetRandomString());

            var processingDependencyValidationException =
                new CareIdentityServiceProcessingDependencyValidationException(
                    GetRandomString(),
                    innerException);

            SetupEveryOperationToThrow(processingDependencyValidationException);

            // when
            Exception actualException = await InvokeOperationAsync(operation);

            // then
            actualException.Should().BeOfType<CareIdentityServiceClientDependencyValidationException>();
            actualException.InnerException.Should().BeSameAs(innerException);
        }

        [Theory]
        [MemberData(nameof(ClientOperations))]
        public async Task ShouldThrowClientDependencyExceptionOnEveryOperationIfDependencyErrorOccursAsync(
            string operation)
        {
            // given
            var innerException = new Xeption(message: GetRandomString());

            var processingDependencyException =
                new CareIdentityServiceProcessingDependencyException(GetRandomString(), innerException);

            SetupEveryOperationToThrow(processingDependencyException);

            // when
            Exception actualException = await InvokeOperationAsync(operation);

            // then
            actualException.Should().BeOfType<CareIdentityServiceClientDependencyException>();
            actualException.InnerException.Should().BeSameAs(innerException);
        }

        [Theory]
        [MemberData(nameof(ClientOperations))]
        public async Task ShouldThrowClientServiceExceptionOnEveryOperationIfServiceErrorOccursAsync(
            string operation)
        {
            // given
            var innerException = new Xeption(message: GetRandomString());

            var processingServiceException =
                new CareIdentityServiceProcessingServiceException(GetRandomString(), innerException);

            SetupEveryOperationToThrow(processingServiceException);

            // when
            Exception actualException = await InvokeOperationAsync(operation);

            // then
            actualException.Should().BeOfType<CareIdentityServiceClientServiceException>();
            actualException.InnerException.Should().BeSameAs(innerException);
        }

        [Theory]
        [MemberData(nameof(ClientOperations))]
        public async Task ShouldThrowClientServiceExceptionOnEveryOperationIfUnexpectedErrorOccursAsync(
            string operation)
        {
            // given
            var unexpectedException = new InvalidOperationException(GetRandomString());
            SetupEveryOperationToThrow(unexpectedException);

            // when
            Exception actualException = await InvokeOperationAsync(operation);

            // then
            actualException.Should().BeOfType<CareIdentityServiceClientServiceException>();

            actualException.InnerException
                .Should().BeOfType<FailedCareIdentityServiceClientException>();

            actualException.InnerException.InnerException.Should().BeSameAs(unexpectedException);
        }

        [Theory]
        [MemberData(nameof(ClientOperations))]
        public async Task ShouldRethrowOperationCanceledExceptionOnEveryOperationAsync(string operation)
        {
            // given
            SetupEveryOperationToThrow(new OperationCanceledException());

            // when
            Exception actualException = await InvokeOperationAsync(operation);

            // then
            actualException.Should().BeAssignableTo<OperationCanceledException>();
        }

        private void SetupEveryOperationToThrow(Exception exception)
        {
            this.careIdentityServiceProcessingServiceMock.Setup(service =>
                service.BuildLoginUrlAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(exception);

            this.careIdentityServiceProcessingServiceMock.Setup(service =>
                service.LogoutAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(exception);

            this.careIdentityServiceProcessingServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(exception);

            this.careIdentityServiceProcessingServiceMock.Setup(service =>
                service.GetUserInfoAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(exception);
        }

        private async Task<Exception> InvokeOperationAsync(string operation)
        {
            Exception actualException = await Record.ExceptionAsync(async () =>
            {
                switch (operation)
                {
                    case nameof(ICareIdentityServiceClientOperation.BuildLoginUrl):
                        await this.careIdentityServiceClient.BuildLoginUrlAsync();
                        break;

                    case nameof(ICareIdentityServiceClientOperation.Logout):
                        await this.careIdentityServiceClient.LogoutAsync();
                        break;

                    case nameof(ICareIdentityServiceClientOperation.GetAccessToken):
                        await this.careIdentityServiceClient.GetAccessTokenAsync();
                        break;

                    default:
                        await this.careIdentityServiceClient.GetUserInfoAsync(
                            GetRandomString(),
                            GetRandomString());

                        break;
                }
            });

            actualException.Should().NotBeNull(
                "every operation must surface the failure raised by the processing service");

            return actualException;
        }

        // Names only - gives the theory data compile-time safety against a renamed client operation.
        private interface ICareIdentityServiceClientOperation
        {
            void BuildLoginUrl();
            void Logout();
            void GetAccessToken();
            void GetUserInfo();
        }
    }
}
