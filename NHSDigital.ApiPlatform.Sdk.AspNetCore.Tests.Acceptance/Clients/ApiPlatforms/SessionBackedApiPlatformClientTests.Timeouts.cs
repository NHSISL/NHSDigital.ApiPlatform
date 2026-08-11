// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NHSDigital.ApiPlatform.Sdk.Clients.ApiPlatforms;
using NHSDigital.ApiPlatform.Sdk.Models.Clients.CareIdentityService.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Clients.Pds.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.AspNetCore.Tests.Acceptance.Clients.ApiPlatforms
{
    public partial class SessionBackedApiPlatformClientTests
    {
        // Only these two tests need a short dependency timeout. Applying it to the whole class
        // would leave every other test one slow HTTP call away from failing as a timeout.
        private static readonly TimeSpan ShortDependencyTimeout = TimeSpan.FromSeconds(1);

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnGetUserInfoIfTheTokenEndpointTimesOutAsync()
        {
            // given
            this.wireMockServer
                .Given(Request.Create().WithPath(TokenPath).UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithDelay(TimeSpan.FromSeconds(5))
                    .WithBody("{}"));

            using ServiceProvider timeoutProvider = BuildServiceProvider(ShortDependencyTimeout);
            using IServiceScope timeoutScope = timeoutProvider.CreateScope();

            IApiPlatformClient timeoutClient =
                timeoutScope.ServiceProvider.GetRequiredService<IApiPlatformClient>();

            string loginUrl = await timeoutClient.CareIdentityServiceClient.BuildLoginUrlAsync();
            string state = ExtractStateFromLoginUrl(loginUrl);

            // when
            CareIdentityServiceClientDependencyException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceClientDependencyException>(async () =>
                    await timeoutClient.CareIdentityServiceClient.GetUserInfoAsync(
                        GetRandomString(),
                        state));

            // then
            actualException.InnerException.InnerException.Should().BeOfType<TimeoutException>();

            actualException.InnerException.InnerException.Message
                .Should().Be("The dependency operation timed out.");
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnSearchPatientsIfThePatientEndpointTimesOutAsync()
        {
            // given
            string randomNhsNumber = GetRandomNhsNumber();
            using ServiceProvider timeoutProvider = BuildServiceProvider(ShortDependencyTimeout);
            using IServiceScope timeoutScope = timeoutProvider.CreateScope();

            IApiPlatformClient timeoutClient =
                timeoutScope.ServiceProvider.GetRequiredService<IApiPlatformClient>();

            GivenTokenEndpointReturns(GetRandomString(), GetRandomString());
            GivenUserInfoEndpointReturns(GetRandomString(), GetRandomString());
            string loginUrl = await timeoutClient.CareIdentityServiceClient.BuildLoginUrlAsync();
            string state = ExtractStateFromLoginUrl(loginUrl);
            await timeoutClient.CareIdentityServiceClient.GetUserInfoAsync(GetRandomString(), state);

            this.wireMockServer
                .Given(Request.Create().WithPath($"{FhirPath}/Patient/{randomNhsNumber}").UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithDelay(TimeSpan.FromSeconds(5))
                    .WithBody("{}"));

            SearchCriteria searchCriteria = CreateSearchCriteriaByNhsNumber(randomNhsNumber);

            // when
            PersonalDemographicsServiceClientDependencyException actualException =
                await Assert.ThrowsAsync<PersonalDemographicsServiceClientDependencyException>(async () =>
                    await timeoutClient.PersonalDemographicsServiceClient.SearchPatientsAsync(
                        searchCriteria));

            // then
            actualException.InnerException.InnerException.Should().BeOfType<TimeoutException>();
        }
    }
}
