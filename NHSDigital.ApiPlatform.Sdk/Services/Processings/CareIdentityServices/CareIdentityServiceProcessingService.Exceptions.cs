// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Models.Processings.CareIdentityServices.Exceptions;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Services.Processings.CareIdentityServices
{
    internal partial class CareIdentityServiceProcessingService : ICareIdentityServiceProcessingService
    {
        private delegate ValueTask<T> ReturningTaskFunction<T>();
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask<T> TryCatch<T>(ReturningTaskFunction<T> returningTaskFunction)
        {
            try
            {
                return await returningTaskFunction();
            }
            catch (InvalidArgumentCareIdentityServiceProcessingException
                invalidArgumentCareIdentityServiceProcessingException)
            {
                throw await CreateValidationExceptionAsync(invalidArgumentCareIdentityServiceProcessingException);
            }
            catch (UnauthorisedCareIdentityServiceProcessingException
                unauthorisedCareIdentityServiceProcessingException)
            {
                throw await CreateValidationExceptionAsync(unauthorisedCareIdentityServiceProcessingException);
            }
            catch (Exception exception)
            {
                var failedCareIdentityServiceProcessingException =
                    new FailedCareIdentityServiceProcessingException(
                        message: "Failed care identity service processing error occurred, please contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateServiceExceptionAsync(failedCareIdentityServiceProcessingException);
            }
        }

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (Exception exception)
            {
                var failedCareIdentityServiceProcessingException =
                    new FailedCareIdentityServiceProcessingException(
                        message: "Failed care identity service processing error occurred, please contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateServiceExceptionAsync(failedCareIdentityServiceProcessingException);
            }
        }

        private async ValueTask<CareIdentityServiceProcessingValidationException> CreateValidationExceptionAsync(
            Xeption exception)
        {
            var careIdentityServiceProcessingValidationException = new CareIdentityServiceProcessingValidationException(
                message: "Care identity service processing validation error occurred, " +
                    "please fix the errors and try again.",

                innerException: exception);

            return careIdentityServiceProcessingValidationException;
        }

        private async ValueTask<CareIdentityServiceProcessingServiceException> CreateServiceExceptionAsync(
            Xeption exception)
        {
            var careIdentityServiceProcessingServiceException = new CareIdentityServiceProcessingServiceException(
                message: "Care identity service processing error occurred, please contact support.",
                innerException: exception);

            return careIdentityServiceProcessingServiceException;
        }
    }
}
