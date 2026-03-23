// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using NHSDigital.ApiPlatform.Sdk.Models.Processings.CareIdentityServices.Exceptions;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Services.Processings.CareIdentityServices
{
    internal partial class CareIdentityServiceProcessingService : ICareIdentityServiceProcessingService
    {
        public void ValidateOnGetUserInfo(string code, string state)
        {
            Validate(
                createException: () => new InvalidArgumentCareIdentityServiceProcessingException(
                    message: "Invalid argument(s), please correct the errors and try again."),

                (Rule: IsInvalid(code), Parameter: nameof(code)),
                (Rule: IsInvalid(state), Parameter: nameof(state)));
        }

        public void ValidateAccessToken(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new UnauthorisedCareIdentityServiceProcessingException(
                    message: "Authentication failed (no access token).");
            }
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
