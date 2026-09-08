// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using NHSDigital.ApiPlatform.Sdk.Models.Clients.Pds.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Acceptance.Clients.ApiPlatforms
{
    public partial class ApiPlatformClientTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnSearchPatientsIfSearchCriteriaIsNullAsync()
        {
            // given
            SearchCriteria nullSearchCriteria = null;

            // when
            // then
            await Assert.ThrowsAsync<PersonalDemographicsServiceClientValidationException>(async () =>
                await this.personalDemographicsServiceClient.SearchPatientsAsync(nullSearchCriteria));
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnSearchPatientsIfSearchCriteriaIsEmptyAsync()
        {
            // given
            var emptySearchCriteria = new SearchCriteria();

            // when
            // then
            await Assert.ThrowsAsync<PersonalDemographicsServiceClientValidationException>(async () =>
                await this.personalDemographicsServiceClient.SearchPatientsAsync(emptySearchCriteria));
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnSearchPatientsIfNotAuthenticatedAsync()
        {
            // given
            SearchCriteria searchCriteria = CreateSearchCriteriaByNhsNumber(GetRandomNhsNumber());

            // when
            PersonalDemographicsServiceClientValidationException actualException =
                await Assert.ThrowsAsync<PersonalDemographicsServiceClientValidationException>(async () =>
                    await this.personalDemographicsServiceClient.SearchPatientsAsync(searchCriteria));

            // then
            actualException.InnerException.Message
                .Should().Be("Unauthorized - Unable to retrieve access token.");
        }
    }
}
