using System;
using System.Threading.Tasks;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Integration
{
    public partial class NhsLoginTests
    {
        [Fact]
        public async Task GetAccessToken()
        {
            // given
            // when
            await careIdentityServiceClient.GetAccessTokenAsync();

            // then
            Assert.True(true, "Logout completed successfully without throwing an exception.");
        }
    }
}