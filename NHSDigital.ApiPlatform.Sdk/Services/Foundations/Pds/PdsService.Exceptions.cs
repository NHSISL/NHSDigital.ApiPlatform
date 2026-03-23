// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.Exceptions;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Services.Foundations.Pds
{
    internal partial class PdsService : IPdsService
    {
        private delegate ValueTask<string> ReturningStringFunction();

        private async ValueTask<string> TryCatch(ReturningStringFunction returningStringFunction)
        {
            try
            {
                return await returningStringFunction();
            }
            catch (InvalidArgumentPdsServiceException invalidArgumentPdsServiceException)
            {
                throw await CreateValidationExceptionAsync(invalidArgumentPdsServiceException);
            }
            //TODO: Extend this to catch dependency and dependency validation exceptions.
            catch (Exception exception)
            {
                var failedPdsServiceException =
                    new FailedPdsServiceException(
                        message: "Failed PDS service error occurred, please contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateServiceExceptionAsync(failedPdsServiceException);
            }
        }

        private async ValueTask<PdsServiceValidationException> CreateValidationExceptionAsync(
            Xeption exception)
        {
            var pdsServiceValidationException = new PdsServiceValidationException(
                message: "PDS service validation error occurred, please fix the errors and try again.",
                innerException: exception);

            return pdsServiceValidationException;
        }

        private async ValueTask<PdsServiceException> CreateServiceExceptionAsync(Xeption exception)
        {
            var pdsServiceException = new PdsServiceException(
                message: "PDS service error occurred, please contact support.",
                innerException: exception);

            return pdsServiceException;
        }
    }
}
