// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------
using System;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices.Exceptions;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Services.Foundations.CareIdentityServices
{
    internal partial class CareIdentityService
    {
        public void ValidateOnCallback(string code, string state)
        {
            Validate(
                createException: () => new InvalidArgumentCareIdentityServiceException(
                    message: "Invalid argument(s), please correct the errors and try again."),

                (Rule: IsInvalid(code), Parameter: nameof(code)),
                (Rule: IsInvalid(state), Parameter: nameof(state)));
        }

        public void ValidateOnExchangeCodeForToken(string code)
        {
            Validate(
                createException: () => new InvalidArgumentCareIdentityServiceException(
                    message: "Invalid argument(s), please correct the errors and try again."),

                (Rule: IsInvalid(code), Parameter: nameof(code)));
        }

        public void ValidateAccessToken(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new UnauthorisedCareIdentityServiceException(
                    message: "Authentication failed (no access token).");
            }
        }

        public void ValidateOnGetUserInfo(string accessToken)
        {
            Validate(
                createException: () => new InvalidArgumentCareIdentityServiceException(
                    message: "Invalid argument(s), please correct the errors and try again."),

                (Rule: IsInvalid(accessToken), Parameter: nameof(accessToken)));
        }

        public void ValidateOnExchangeRefreshTokenForToken(string refreshToken)
        {
            Validate(
                createException: () => new InvalidArgumentCareIdentityServiceException(
                    message: "Invalid argument(s), please correct the errors and try again."),

                (Rule: IsInvalid(refreshToken), Parameter: nameof(refreshToken)));
        }

        private static dynamic IsInvalid(string? text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static void Validate<T>(
            Func<T> createException,
            params (dynamic Rule, string Parameter)[] validations)
            where T : Xeption
        {
            T invalidDataException = createException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidDataException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidDataException.ThrowIfContainsErrors();
        }
    }
}
