// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices.Exceptions;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Foundations.CareIdentityServices
{
    public partial class CareIdentityServiceTests
    {
        [Fact]
        public async Task ShouldPostTheAuthorizationCodeGrantToTheTokenEndpointOnCallbackAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            TokenResult randomTokenResult = CreateRandomTokenResult();
            NhsUserInfo randomUserInfo = CreateRandomNhsUserInfo();
            IEnumerable<KeyValuePair<string, string>> actualFormValues = null;

            SetupSuccessfulCallback(randomState, randomDateTimeOffset, randomTokenResult, randomUserInfo);

            this.httpBrokerMock.Setup(broker =>
                broker.PostFormAsync(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<KeyValuePair<string, string>>>(),
                    It.IsAny<CancellationToken>()))
                        .Callback<string, IEnumerable<KeyValuePair<string, string>>, CancellationToken>(
                            (url, formValues, cancellationToken) => actualFormValues = formValues)
                        .ReturnsAsync(CreateHttpResponse(GetRandomString()));

            this.jsonBrokerMock.Setup(broker =>
                broker.Deserialize<TokenResult>(It.IsAny<string>()))
                    .Returns(randomTokenResult);

            // when
            await this.careIdentityService.CallbackAsync(randomCode, randomState);

            // then
            Dictionary<string, string> actualForm =
                actualFormValues.ToDictionary(pair => pair.Key, pair => pair.Value);

            actualForm["grant_type"].Should().Be("authorization_code");
            actualForm["code"].Should().Be(randomCode);
            actualForm["redirect_uri"].Should().Be(this.apiPlatformConfigurations.CareIdentity.RedirectUri);
            actualForm["client_id"].Should().Be(this.apiPlatformConfigurations.CareIdentity.ClientId);
            actualForm["client_secret"].Should().Be(this.apiPlatformConfigurations.CareIdentity.ClientSecret);
        }

        [Fact]
        public async Task ShouldPostTheRefreshTokenGrantToTheTokenEndpointOnGetAccessTokenAsync()
        {
            // given
            string randomRefreshToken = GetRandomString();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            TokenResult randomTokenResult = CreateRandomTokenResult();
            IEnumerable<KeyValuePair<string, string>> actualFormValues = null;
            string randomTokenJson = GetRandomString();

            this.tokenBrokerMock.Setup(broker =>
                broker.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync((null, null));

            this.tokenBrokerMock.Setup(broker =>
                broker.GetRefreshTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync((randomRefreshToken, randomDateTimeOffset.AddMinutes(30)));

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            this.httpBrokerMock.Setup(broker =>
                broker.PostFormAsync(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<KeyValuePair<string, string>>>(),
                    It.IsAny<CancellationToken>()))
                        .Callback<string, IEnumerable<KeyValuePair<string, string>>, CancellationToken>(
                            (url, formValues, cancellationToken) => actualFormValues = formValues)
                        .ReturnsAsync(CreateHttpResponse(randomTokenJson));

            this.jsonBrokerMock.Setup(broker =>
                broker.Deserialize<TokenResult>(randomTokenJson))
                    .Returns(randomTokenResult);

            // when
            await this.careIdentityService.GetAccessTokenAsync();

            // then
            Dictionary<string, string> actualForm =
                actualFormValues.ToDictionary(pair => pair.Key, pair => pair.Value);

            actualForm["grant_type"].Should().Be("refresh_token");
            actualForm["refresh_token"].Should().Be(randomRefreshToken);
            actualForm["client_id"].Should().Be(this.apiPlatformConfigurations.CareIdentity.ClientId);
            actualForm["client_secret"].Should().Be(this.apiPlatformConfigurations.CareIdentity.ClientSecret);
        }

        [Fact]
        public async Task ShouldSendTheBearerTokenOnGetUserInfoAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            string randomUserInfoJson = GetRandomString();
            var actualRequest = new HttpRequestMessage(HttpMethod.Get, "https://localhost/userinfo");

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .Callback<string, Action<HttpRequestMessage>, CancellationToken>(
                            (url, configureRequest, cancellationToken) => configureRequest(actualRequest))
                        .ReturnsAsync(CreateHttpResponse(randomUserInfoJson));

            this.jsonBrokerMock.Setup(broker =>
                broker.Deserialize<NhsUserInfo>(randomUserInfoJson))
                    .Returns(CreateRandomNhsUserInfo());

            // when
            await this.careIdentityService.GetUserInfoAsync(randomAccessToken, default);

            // then
            actualRequest.Headers.Authorization.Scheme.Should().Be("Bearer");
            actualRequest.Headers.Authorization.Parameter.Should().Be(randomAccessToken);
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.Forbidden)]
        [InlineData(HttpStatusCode.NotFound)]
        public async Task ShouldThrowDependencyValidationExceptionOnGetUserInfoIfDependencyRejectsTheRequestAsync(
            HttpStatusCode statusCode)
        {
            // given
            string randomAccessToken = GetRandomString();

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new HttpResponseMessage(statusCode)
                        {
                            Content = new StringContent(GetRandomString())
                        });

            // when
            ValueTask<NhsUserInfo> getUserInfoTask =
                this.careIdentityService.GetUserInfoAsync(randomAccessToken, default);

            CareIdentityServiceDependencyValidationException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceDependencyValidationException>(
                    async () => await getUserInfoTask);

            // then
            actualException.InnerException
                .Should().BeOfType<InvalidCareIdentityServiceDependencyException>();

            actualException.InnerException.InnerException.Should().BeOfType<HttpRequestException>();
        }

        [Theory]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.BadGateway)]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        public async Task ShouldThrowDependencyExceptionOnGetUserInfoIfDependencyFailsAsync(
            HttpStatusCode statusCode)
        {
            // given
            string randomAccessToken = GetRandomString();

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new HttpResponseMessage(statusCode)
                        {
                            Content = new StringContent(GetRandomString())
                        });

            // when
            ValueTask<NhsUserInfo> getUserInfoTask =
                this.careIdentityService.GetUserInfoAsync(randomAccessToken, default);

            CareIdentityServiceDependencyException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceDependencyException>(
                    async () => await getUserInfoTask);

            // then
            actualException.InnerException
                .Should().BeOfType<FailedCareIdentityServiceDependencyException>();
        }
    }
}
