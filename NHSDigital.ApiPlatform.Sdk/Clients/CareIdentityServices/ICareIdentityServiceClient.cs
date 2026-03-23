// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices;

namespace NHSDigital.ApiPlatform.Sdk.Clients.CareIdentityServices
{
    public interface ICareIdentityServiceClient
    {
        /// <summary>
        /// Logs in to the Care Identity Service.
        /// </summary>
        /// <returns>Returns a redirection URL.</returns>
        ValueTask<string> BuildLoginUrlAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Logs out of the Care Identity Service.
        /// </summary>
        /// <returns>Returns a redirection URL.</returns>
        ValueTask LogoutAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the access token used to authenticate API requests.
        /// </summary>
        /// <returns>A string containing the access token required for authorized API calls.</returns>
        ValueTask<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the current CIS2 user information for the provided access token.
        /// </summary>
        /// <param name="code">
        /// The code to be processed by the callback.
        /// Typically represents an authorization or verification code
        /// received from an external source.
        /// </param>
        /// <param name="state">
        /// The state information associated with the callback.
        /// Used to maintain context or verify the integrity of the operation.
        /// </param>
        /// <returns>The user information associated with the access token.</returns>
        ValueTask<NhsUserInfo> GetUserInfoAsync(
            string code,
            string state,
            CancellationToken cancellationToken = default);
    }
}
