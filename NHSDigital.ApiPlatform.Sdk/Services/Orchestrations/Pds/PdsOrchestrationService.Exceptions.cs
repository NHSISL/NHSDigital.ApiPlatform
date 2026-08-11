// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Orchestrations.Pds.Exceptions;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Services.Orchestrations.Pds
{
    internal sealed partial class PdsOrchestrationService
    {
        private delegate ValueTask<string> ReturningStringFunction();

        private async ValueTask<string> TryCatch(
            ReturningStringFunction returningStringFunction,
            CancellationToken cancellationToken)
        {
            try
            {
                return await returningStringFunction();
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested is false)
            {
                throw await CreateAndLogTimeoutDependencyExceptionAsync();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullSearchCriteriaPdsOrchestrationException nullSearchCriteriaPdsOrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullSearchCriteriaPdsOrchestrationException);
            }
            catch (InvalidArgumentPdsOrchestrationException invalidArgumentPdsOrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidArgumentPdsOrchestrationException);
            }
            catch (UnauthorizedPdsOrchestrationException unauthorizedPdsOrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(unauthorizedPdsOrchestrationException);
            }
            catch (PdsOrchestrationValidationException)
            {
                throw;
            }
            catch (PdsOrchestrationDependencyValidationException)
            {
                throw;
            }
            catch (PdsOrchestrationDependencyException)
            {
                throw;
            }
            catch (PdsOrchestrationServiceException)
            {
                throw;
            }
            catch (CareIdentityServiceValidationException careIdentityValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(careIdentityValidationException);
            }
            catch (CareIdentityServiceDependencyValidationException careIdentityDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(careIdentityDependencyValidationException);
            }
            catch (CareIdentityServiceDependencyException careIdentityServiceDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(careIdentityServiceDependencyException);
            }
            catch (CareIdentityServiceServiceException careIdentityServiceServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(careIdentityServiceServiceException);
            }
            catch (PdsServiceValidationException pdsServiceValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(pdsServiceValidationException);
            }
            catch (PdsServiceDependencyValidationException pdsServiceDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(pdsServiceDependencyValidationException);
            }
            catch (PdsServiceDependencyException pdsServiceDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(pdsServiceDependencyException);
            }
            catch (PdsServiceException pdsServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(pdsServiceException);
            }
            catch (TimeoutException timeoutException)
            {
                var timeoutPdsOrchestrationException =
                    new TimeoutPdsOrchestrationException(
                        message: "Failed PDS orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutExceptionAsync(timeoutPdsOrchestrationException);
            }
            catch (Exception exception)
            {
                var failedPdsOrchestrationException =
                    new FailedPdsOrchestrationException(
                        message: "Failed PDS orchestration service error occurred, please contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedPdsOrchestrationException);
            }
        }

        private async ValueTask<PdsOrchestrationDependencyException> CreateAndLogTimeoutDependencyExceptionAsync()
        {
            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutPdsOrchestrationException =
                new TimeoutPdsOrchestrationException(
                    message: "Failed PDS orchestration timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            return await CreateAndLogTimeoutExceptionAsync(timeoutPdsOrchestrationException);
        }

        private async ValueTask<PdsOrchestrationDependencyException> CreateAndLogTimeoutExceptionAsync(
            Xeption exception)
        {
            var pdsOrchestrationDependencyException =
                new PdsOrchestrationDependencyException(
                    message: "PDS orchestration dependency error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(pdsOrchestrationDependencyException);

            return pdsOrchestrationDependencyException;
        }

        private async ValueTask<PdsOrchestrationValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var pdsOrchestrationValidationException =
                new PdsOrchestrationValidationException(
                    message: "PDS orchestration validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(pdsOrchestrationValidationException);

            return pdsOrchestrationValidationException;
        }

        private async ValueTask<PdsOrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var pdsOrchestrationDependencyValidationException =
                new PdsOrchestrationDependencyValidationException(
                    message: "PDS orchestration dependency validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(pdsOrchestrationDependencyValidationException);

            return pdsOrchestrationDependencyValidationException;
        }

        private async ValueTask<PdsOrchestrationDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var pdsOrchestrationDependencyException =
                new PdsOrchestrationDependencyException(
                    message: "PDS orchestration dependency error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(pdsOrchestrationDependencyException);

            return pdsOrchestrationDependencyException;
        }

        private async ValueTask<PdsOrchestrationServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var pdsOrchestrationServiceException = new PdsOrchestrationServiceException(
                message: "PDS orchestration service error occurred, please contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(pdsOrchestrationServiceException);

            return pdsOrchestrationServiceException;
        }
    }
}
