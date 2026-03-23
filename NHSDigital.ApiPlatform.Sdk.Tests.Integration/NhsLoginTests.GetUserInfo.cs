using System;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Integration
{
    public partial class NhsLoginTests
    {
        [Fact(Skip = "Requires real NHS authentication flow with valid authorization code")]
        public async Task GetUserInfo()
        {
            // given
            string code = "test-authorization-code";
            string state = "test-state-value";

            // when
            NhsUserInfo userInfo =
                await careIdentityServiceClient.GetUserInfoAsync(
                    code,
                    state);

            // then
            Assert.NotNull(userInfo);
        }
    }
}