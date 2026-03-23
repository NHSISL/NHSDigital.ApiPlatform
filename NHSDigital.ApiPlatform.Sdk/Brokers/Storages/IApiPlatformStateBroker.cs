// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------
using System.Threading;
using System.Threading.Tasks;

namespace NHSDigital.ApiPlatform.Sdk.Brokers.Storages
{
    public interface IApiPlatformStateBroker
    {
        ValueTask StoreCsrfStateAsync(string state, CancellationToken cancellationToken = default);
        ValueTask<string?> GetCsrfStateAsync(CancellationToken cancellationToken = default);
        ValueTask ClearCsrfStateAsync(CancellationToken cancellationToken = default);
    }
}
