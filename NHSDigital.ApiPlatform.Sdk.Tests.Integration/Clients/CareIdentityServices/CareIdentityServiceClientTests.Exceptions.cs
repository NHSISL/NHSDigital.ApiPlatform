// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NHSDigital.ApiPlatform.Sdk.Models.Clients.CareIdentityService.Exceptions;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Integration.Clients.CareIdentityServices
{
    public partial class CareIdentityServiceClientTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnGetUserInfoIfStateWasNeverIssuedAsync()
        {
            // given
            string unknownState = GetRandomString();
            string authorisationCode = GetRandomString();

            // when
            CareIdentityServiceClientDependencyValidationException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceClientDependencyValidationException>(async () =>
                    await this.careIdentityServiceClient.GetUserInfoAsync(authorisationCode, unknownState));

            // then
            actualException.InnerException.Message.Should().Be("Invalid state parameter.");
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnGetUserInfoIfCodeIsMissingAsync()
        {
            // given
            string emptyCode = string.Empty;
            string randomState = GetRandomString();

            // when
            // then
            await Assert.ThrowsAsync<CareIdentityServiceClientValidationException>(async () =>
                await this.careIdentityServiceClient.GetUserInfoAsync(emptyCode, randomState));
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionOnBuildLoginUrlIfTokenIsAlreadyCancelledAsync()
        {
            // given
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // when
            // then
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
                await this.careIdentityServiceClient.BuildLoginUrlAsync(cancellationTokenSource.Token));
        }

        [Fact(Skip = "Requires NHS CIS2 credentials and reaches the live INT token endpoint.")]
        public async Task ShouldThrowDependencyExceptionOnGetUserInfoIfAuthorisationCodeIsRejectedAsync()
        {
            // given
            string loginUrl = await this.careIdentityServiceClient.BuildLoginUrlAsync();
            string state = ExtractQueryValue(loginUrl, "state");
            string rejectedCode = GetRandomString();

            // when
            // then
            await Assert.ThrowsAsync<CareIdentityServiceClientDependencyException>(async () =>
                await this.careIdentityServiceClient.GetUserInfoAsync(rejectedCode, state));
        }
    }
}
