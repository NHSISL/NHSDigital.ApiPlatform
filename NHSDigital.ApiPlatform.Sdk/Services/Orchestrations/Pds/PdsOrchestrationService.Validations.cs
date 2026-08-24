// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using NHSDigital.ApiPlatform.Sdk.Models.Orchestrations.Pds.Exceptions;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Services.Orchestrations.Pds
{
    internal sealed partial class PdsOrchestrationService
    {
        public void ValidateOnSearchPatients(SearchCriteria searchCriteria)
        {
            ValidateSearchCriteriaIsNotNull(searchCriteria);

            Validate(
                createException: () => new InvalidArgumentPdsOrchestrationException(
                    message: "Invalid argument(s), please correct the errors and try again."),

                (Rule: IsInvalidSearchCriteria(searchCriteria), Parameter: nameof(searchCriteria)));
        }

        public void ValidateAccessToken(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new UnauthorizedPdsOrchestrationException(
                    message: "Unauthorized - Unable to retrieve access token.");
            }
        }

        private static void ValidateSearchCriteriaIsNotNull(SearchCriteria searchCriteria)
        {
            if (searchCriteria is null)
            {
                throw new NullSearchCriteriaPdsOrchestrationException(
                    message: "Search criteria is null.");
            }
        }

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
