using System;
using System.Threading.Tasks;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Integration
{
    public partial class NhsLoginTests
    {
        [Fact]
        public async Task BuildLoginUrl()
        {
            // given
            // when
            string loginUrl = await careIdentityServiceClient.BuildLoginUrlAsync();

            // then
            Assert.False(string.IsNullOrWhiteSpace(loginUrl), "Login URL should not be null or empty.");
            Assert.Contains(apiPlatformConfigurations.CareIdentity.AuthEndpoint, loginUrl);
        }
    }
}