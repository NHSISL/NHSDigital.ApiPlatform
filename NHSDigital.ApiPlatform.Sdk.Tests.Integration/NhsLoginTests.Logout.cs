using System;
using System.Threading.Tasks;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Integration
{
    public partial class NhsLoginTests
    {
        [Fact]
        public async Task Logout()
        {
            // given
            // when
            await careIdentityServiceClient.LogoutAsync();

            // then
            Assert.True(true, "Logout completed successfully without throwing an exception.");
        }
    }
}