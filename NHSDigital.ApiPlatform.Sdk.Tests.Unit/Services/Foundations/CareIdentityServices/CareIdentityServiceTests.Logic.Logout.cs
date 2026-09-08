// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Foundations.CareIdentityServices
{
    public partial class CareIdentityServiceTests
    {
        [Fact]
        public async Task ShouldClearCsrfStateOnLogoutAsync()
        {
            // given
            // when
            await this.careIdentityService.LogoutAsync();

            // then
            this.stateBrokerMock.Verify(broker =>
                broker.ClearCsrfStateAsync(It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Fact]
        public async Task ShouldClearAccessTokenOnLogoutAsync()
        {
            // given
            // when
            await this.careIdentityService.LogoutAsync();

            // then
            this.tokenBrokerMock.Verify(broker =>
                broker.ClearAccessTokenAsync(It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Fact]
        public async Task ShouldClearRefreshTokenOnLogoutAsync()
        {
            // given
            // when
            await this.careIdentityService.LogoutAsync();

            // then
            this.tokenBrokerMock.Verify(broker =>
                broker.ClearRefreshTokenAsync(It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Fact]
        public async Task ShouldClearActiveRoleOnLogoutAsync()
        {
            // given
            // when
            await this.careIdentityService.LogoutAsync();

            // then
            this.tokenBrokerMock.Verify(broker =>
                broker.ClearActiveRoleAsync(It.IsAny<CancellationToken>()),
                    Times.Once);
        }
    }
}
