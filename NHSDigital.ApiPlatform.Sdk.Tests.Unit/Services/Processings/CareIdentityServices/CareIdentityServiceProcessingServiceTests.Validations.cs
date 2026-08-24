// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices;
using NHSDigital.ApiPlatform.Sdk.Models.Processings.CareIdentityServices.Exceptions;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Processings.CareIdentityServices
{
    public partial class CareIdentityServiceProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(InvalidTexts))]
        public async Task ShouldThrowValidationExceptionOnGetUserInfoIfCodeIsInvalidAsync(string invalidCode)
        {
            // given
            string randomState = GetRandomString();

            var invalidArgumentCareIdentityServiceProcessingException =
                new InvalidArgumentCareIdentityServiceProcessingException(
                    message: "Invalid argument(s), please correct the errors and try again.");

            invalidArgumentCareIdentityServiceProcessingException.UpsertDataList(
                key: "code",
                value: "Text is required");

            var expectedException =
                new CareIdentityServiceProcessingValidationException(
                    message: "Care identity service processing validation error occurred, " +
                        "please fix the errors and try again.",

                    innerException: invalidArgumentCareIdentityServiceProcessingException);

            // when
            ValueTask<NhsUserInfo> getUserInfoTask =
                this.careIdentityServiceProcessingService.GetUserInfoAsync(invalidCode, randomState);

            CareIdentityServiceProcessingValidationException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceProcessingValidationException>(
                    async () => await getUserInfoTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.careIdentityServiceMock.Verify(service =>
                service.CallbackAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);
        }

        [Theory]
        [MemberData(nameof(InvalidTexts))]
        public async Task ShouldThrowValidationExceptionOnGetUserInfoIfStateIsInvalidAsync(string invalidState)
        {
            // given
            string randomCode = GetRandomString();

            var invalidArgumentCareIdentityServiceProcessingException =
                new InvalidArgumentCareIdentityServiceProcessingException(
                    message: "Invalid argument(s), please correct the errors and try again.");

            invalidArgumentCareIdentityServiceProcessingException.UpsertDataList(
                key: "state",
                value: "Text is required");

            var expectedException =
                new CareIdentityServiceProcessingValidationException(
                    message: "Care identity service processing validation error occurred, " +
                        "please fix the errors and try again.",

                    innerException: invalidArgumentCareIdentityServiceProcessingException);

            // when
            ValueTask<NhsUserInfo> getUserInfoTask =
                this.careIdentityServiceProcessingService.GetUserInfoAsync(randomCode, invalidState);

            CareIdentityServiceProcessingValidationException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceProcessingValidationException>(
                    async () => await getUserInfoTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);
        }

        [Theory]
        [MemberData(nameof(InvalidTexts))]
        public async Task ShouldThrowValidationExceptionOnGetUserInfoIfAccessTokenIsUnavailableAsync(
            string invalidAccessToken)
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();

            var unauthorisedCareIdentityServiceProcessingException =
                new UnauthorisedCareIdentityServiceProcessingException(
                    message: "Authentication failed (no access token).");

            var expectedException =
                new CareIdentityServiceProcessingValidationException(
                    message: "Care identity service processing validation error occurred, " +
                        "please fix the errors and try again.",

                    innerException: unauthorisedCareIdentityServiceProcessingException);

            this.careIdentityServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(invalidAccessToken);

            // when
            ValueTask<NhsUserInfo> getUserInfoTask =
                this.careIdentityServiceProcessingService.GetUserInfoAsync(randomCode, randomState);

            CareIdentityServiceProcessingValidationException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceProcessingValidationException>(
                    async () => await getUserInfoTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.careIdentityServiceMock.Verify(service =>
                service.GetUserInfoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                    Times.Never);
        }
    }
}
