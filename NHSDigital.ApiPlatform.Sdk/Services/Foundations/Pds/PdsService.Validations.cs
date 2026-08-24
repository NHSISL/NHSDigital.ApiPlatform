// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Text.RegularExpressions;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.Exceptions;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Services.Foundations.Pds
{
    internal partial class PdsService : IPdsService
    {
        private static readonly Regex NhsNumberPattern = new Regex(@"^\d{10}$", RegexOptions.Compiled);

        public void ValidateOnSearchPatients(string accessToken, SearchCriteria searchCriteria)
        {
            ValidateSearchCriteriaIsNotNull(searchCriteria);

            Validate(
                createException: () => new InvalidArgumentPdsServiceException(
                    message: "Invalid argument(s), please correct the errors and try again."),

                (Rule: IsInvalid(accessToken), Parameter: nameof(accessToken)),
                (Rule: IsInvalidSearchCriteria(searchCriteria), Parameter: nameof(searchCriteria)),
                (Rule: IsInvalidNhsNumber(searchCriteria.NhsNumber), Parameter: "searchCriteria.NhsNumber"));
        }

        private static void ValidateSearchCriteriaIsNotNull(SearchCriteria searchCriteria)
        {
            if (searchCriteria is null)
            {
                throw new NullSearchCriteriaPdsServiceException(
                    message: "Search criteria is null.");
            }
        }

        private static dynamic IsInvalid(string text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        // An NHS number goes into the request PATH, so escaping alone is not enough: "." and ".." are
        // unreserved characters that survive Uri.EscapeDataString, and Uri parsing then collapses the
        // dot segments, silently moving the request off /Patient. Constraining it to the 10 digits an
        // NHS number actually is closes that off at the source.
        private static dynamic IsInvalidNhsNumber(string nhsNumber) => new
        {
            Condition =
                string.IsNullOrWhiteSpace(nhsNumber) is false &&
                NhsNumberPattern.IsMatch(nhsNumber) is false,

            Message = "NHS number must be 10 digits"
        };

        private static dynamic IsInvalidSearchCriteria(SearchCriteria searchCriteria) => new
        {
            Condition =
                string.IsNullOrWhiteSpace(searchCriteria.NhsNumber) &&
                string.IsNullOrWhiteSpace(searchCriteria.Surname),

            Message = "Either an NHS number or a surname is required"
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
