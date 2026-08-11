// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Acceptance.Clients.ApiPlatforms
{
    public partial class ApiPlatformClientTests
    {
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

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionOnSearchPatientsIfTokenIsAlreadyCancelledAsync()
        {
            // given
            SearchCriteria searchCriteria = CreateSearchCriteriaByNhsNumber(GetRandomNhsNumber());
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // when
            // then
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
                await this.personalDemographicsServiceClient.SearchPatientsAsync(
                    searchCriteria,
                    cancellationTokenSource.Token));
        }

        [Fact]
        public async Task ShouldNotWrapCancellationWhenTheDependencyIsStillRespondingOnSearchPatientsAsync()
        {
            // given
            string randomNhsNumber = GetRandomNhsNumber();
            await GivenAnAuthenticatedSessionAsync();

            this.wireMockServer
                .Given(Request.Create().WithPath($"{FhirPath}/Patient/{randomNhsNumber}").UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithDelay(TimeSpan.FromSeconds(30))
                    .WithBody("{}"));

            SearchCriteria searchCriteria = CreateSearchCriteriaByNhsNumber(randomNhsNumber);
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(250));

            // when
            // then
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
                await this.personalDemographicsServiceClient.SearchPatientsAsync(
                    searchCriteria,
                    cancellationTokenSource.Token));
        }
    }
}
