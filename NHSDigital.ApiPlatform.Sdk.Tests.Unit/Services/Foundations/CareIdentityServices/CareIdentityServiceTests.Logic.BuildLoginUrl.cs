// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Foundations.CareIdentityServices
{
    public partial class CareIdentityServiceTests
    {
        [Fact]
        public async Task ShouldBuildLoginUrlAsync()
        {
            // given
            string randomCsrfState = GetRandomString();

            this.cryptoBrokerMock.Setup(broker =>
                broker.CreateUrlSafeState(It.IsAny<int>()))
                    .Returns(randomCsrfState);

            string expectedLoginUrl =
                $"{this.apiPlatformConfigurations.CareIdentity.AuthEndpoint}" +
                $"?client_id={this.apiPlatformConfigurations.CareIdentity.ClientId}" +
                $"&redirect_uri=" +
                $"{System.Uri.EscapeDataString(this.apiPlatformConfigurations.CareIdentity.RedirectUri)}" +
                $"&response_type=code" +
                $"&state={randomCsrfState}";

            // when
            string actualLoginUrl = await this.careIdentityService.BuildLoginUrlAsync();

            // then
            actualLoginUrl.Should().Be(expectedLoginUrl);
        }

        [Fact]
        public async Task ShouldStoreCsrfStateOnBuildLoginUrlAsync()
        {
            // given
            string randomCsrfState = GetRandomString();

            this.cryptoBrokerMock.Setup(broker =>
                broker.CreateUrlSafeState(It.IsAny<int>()))
                    .Returns(randomCsrfState);

            // when
            await this.careIdentityService.BuildLoginUrlAsync();

            // then
            this.stateBrokerMock.Verify(broker =>
                broker.StoreCsrfStateAsync(randomCsrfState, It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Fact]
        public async Task ShouldAppendAcrValuesOnBuildLoginUrlIfAcrValuesAreConfiguredAsync()
        {
            // given
            string randomAcrValues = GetRandomString();
            this.apiPlatformConfigurations.CareIdentity.AcrValues = randomAcrValues;

            this.cryptoBrokerMock.Setup(broker =>
                broker.CreateUrlSafeState(It.IsAny<int>()))
                    .Returns(GetRandomString());

            // when
            string actualLoginUrl = await this.careIdentityService.BuildLoginUrlAsync();

            // then
            actualLoginUrl.Should().EndWith($"&acr_values={randomAcrValues}");
        }
    }
}
