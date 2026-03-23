// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Models.Clients.CareIdentityService.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices;
using NHSDigital.ApiPlatform.Sdk.Models.Processings.CareIdentityServices.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Services.Processings.CareIdentityServices;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Clients.CareIdentityServices
{
    internal sealed class CareIdentityServiceClient : ICareIdentityServiceClient
    {
        private readonly ICareIdentityServiceProcessingService careIdentityServiceProcessingService;

        public CareIdentityServiceClient(ICareIdentityServiceProcessingService careIdentityServiceProcessingService) =>
            this.careIdentityServiceProcessingService = careIdentityServiceProcessingService;

        public async ValueTask<string> BuildLoginUrlAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.careIdentityServiceProcessingService.BuildLoginUrlAsync(cancellationToken);
            }
            catch (CareIdentityServiceProcessingValidationException careIdentityServiceProcessingValidationException)
            {
                throw CreateCareIdentityServiceClientValidationException(
                    careIdentityServiceProcessingValidationException.InnerException as Xeption);
            }
            catch (CareIdentityServiceProcessingDependencyValidationException
                careIdentityServiceProcessingDependencyValidationException)
            {
                throw CreateCareIdentityServiceClientValidationException(
                    careIdentityServiceProcessingDependencyValidationException.InnerException as Xeption);
            }
            catch (CareIdentityServiceProcessingDependencyException careIdentityServiceProcessingDependencyException)
            {
                throw CreateCareIdentityServiceClientDependencyException(
                    careIdentityServiceProcessingDependencyException.InnerException as Xeption);
            }
            catch (CareIdentityServiceProcessingServiceException careIdentityServiceProcessingServiceException)
            {
                throw CreateCareIdentityServiceClientServiceException(
                    careIdentityServiceProcessingServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateCareIdentityServiceClientServiceException(
                    new FailedCareIdentityServiceClientException(
                        message: "Unexpected error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data));
            }
        }

        public async ValueTask LogoutAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await this.careIdentityServiceProcessingService.LogoutAsync(cancellationToken);
            }
            catch (CareIdentityServiceProcessingValidationException careIdentityServiceProcessingValidationException)
            {
                throw CreateCareIdentityServiceClientValidationException(
                    careIdentityServiceProcessingValidationException.InnerException as Xeption);
            }
            catch (CareIdentityServiceProcessingDependencyValidationException
                careIdentityServiceProcessingDependencyValidationException)
            {
                throw CreateCareIdentityServiceClientValidationException(
                    careIdentityServiceProcessingDependencyValidationException.InnerException as Xeption);
            }
            catch (CareIdentityServiceProcessingDependencyException careIdentityServiceProcessingDependencyException)
            {
                throw CreateCareIdentityServiceClientDependencyException(
                    careIdentityServiceProcessingDependencyException.InnerException as Xeption);
            }
            catch (CareIdentityServiceProcessingServiceException careIdentityServiceProcessingServiceException)
            {
                throw CreateCareIdentityServiceClientServiceException(
                    careIdentityServiceProcessingServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateCareIdentityServiceClientServiceException(
                    new FailedCareIdentityServiceClientException(
                        message: "Unexpected error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data));
            }
        }

        public async ValueTask<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.careIdentityServiceProcessingService.GetAccessTokenAsync(cancellationToken);
            }
            catch (CareIdentityServiceProcessingValidationException careIdentityServiceProcessingValidationException)
            {
                throw CreateCareIdentityServiceClientValidationException(
                    careIdentityServiceProcessingValidationException.InnerException as Xeption);
            }
            catch (CareIdentityServiceProcessingDependencyValidationException
                careIdentityServiceProcessingDependencyValidationException)
            {
                throw CreateCareIdentityServiceClientValidationException(
                    careIdentityServiceProcessingDependencyValidationException.InnerException as Xeption);
            }
            catch (CareIdentityServiceProcessingDependencyException careIdentityServiceProcessingDependencyException)
            {
                throw CreateCareIdentityServiceClientDependencyException(
                    careIdentityServiceProcessingDependencyException.InnerException as Xeption);
            }
            catch (CareIdentityServiceProcessingServiceException careIdentityServiceProcessingServiceException)
            {
                throw CreateCareIdentityServiceClientServiceException(
                    careIdentityServiceProcessingServiceException.InnerException as Xeption);
            }
        }

        public async ValueTask<NhsUserInfo> GetUserInfoAsync(
            string code,
            string state,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.careIdentityServiceProcessingService.GetUserInfoAsync(code, state, cancellationToken);
            }
            catch (CareIdentityServiceProcessingValidationException careIdentityServiceProcessingValidationException)
            {
                throw CreateCareIdentityServiceClientValidationException(
                    careIdentityServiceProcessingValidationException.InnerException as Xeption);
            }
            catch (CareIdentityServiceProcessingDependencyValidationException
                careIdentityServiceProcessingDependencyValidationException)
            {
                throw CreateCareIdentityServiceClientValidationException(
                    careIdentityServiceProcessingDependencyValidationException.InnerException as Xeption);
            }
            catch (CareIdentityServiceProcessingDependencyException careIdentityServiceProcessingDependencyException)
            {
                throw CreateCareIdentityServiceClientDependencyException(
                    careIdentityServiceProcessingDependencyException.InnerException as Xeption);
            }
            catch (CareIdentityServiceProcessingServiceException careIdentityServiceProcessingServiceException)
            {
                throw CreateCareIdentityServiceClientServiceException(
                    careIdentityServiceProcessingServiceException.InnerException as Xeption);
            }
        }

        private static CareIdentityServiceClientValidationException
            CreateCareIdentityServiceClientValidationException(Xeption innerException)
        {
            return new CareIdentityServiceClientValidationException(
                message: "Care identity service client validation error occurred, fix errors and try again.",
                innerException);
        }

        private static CareIdentityServiceClientDependencyException
            CreateCareIdentityServiceClientDependencyException(Xeption innerException)
        {
            return new CareIdentityServiceClientDependencyException(
                message: "Care identity service client dependency error occurred, contact support.",
                innerException);
        }

        private static CareIdentityServiceClientServiceException
            CreateCareIdentityServiceClientServiceException(Xeption innerException)
        {
            return new CareIdentityServiceClientServiceException(
                message: "Care identity service client service error occurred, contact support.",
                innerException);
        }
    }
}
