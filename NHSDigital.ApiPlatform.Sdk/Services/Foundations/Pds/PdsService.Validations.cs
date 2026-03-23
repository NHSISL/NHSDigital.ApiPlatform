// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.Exceptions;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Services.Foundations.Pds
{
    internal partial class PdsService : IPdsService
    {
        public void ValidateOnSearchPatientsAsync(
            string accessToken,
            string family,
            IEnumerable<string> given,
            string gender,
            DateOnly? birthdate)
        {
            Validate(
                createException: () => new InvalidArgumentPdsServiceException(
                    message: "Invalid argument(s), please correct the errors and try again."),

                (Rule: IsInvalid(family), Parameter: nameof(family)),
                (Rule: IsInvalid(given), Parameter: nameof(given)),
                (Rule: IsInvalid(gender), Parameter: nameof(gender)),
                (Rule: IsInvalid(birthdate), Parameter: nameof(birthdate)));
        }

        private static dynamic IsInvalid(string? text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static dynamic IsInvalid(DateOnly? dateOnly) => new
        {
            Condition = dateOnly == null,
            Message = "Date is required"
        };

        private static dynamic IsInvalid(IEnumerable<string> textList) => new
        {
            Condition = textList != null &&
                        textList.Any(text => string.IsNullOrWhiteSpace(text)),

            Message = "List contains null or whitespace values"
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
