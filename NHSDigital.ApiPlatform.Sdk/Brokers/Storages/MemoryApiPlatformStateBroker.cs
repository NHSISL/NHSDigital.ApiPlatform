// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------
using System.Threading;
using System.Threading.Tasks;

namespace NHSDigital.ApiPlatform.Sdk.Brokers.Storages
{
    internal sealed class MemoryApiPlatformStateBroker : IApiPlatformStateBroker
    {
        private readonly object locker = new();
        private string? csrfState;

        public ValueTask StoreCsrfStateAsync(string state, CancellationToken cancellationToken = default)
        {
            lock (this.locker)
            {
                this.csrfState = state;
            }

            return ValueTask.CompletedTask;
        }

        public ValueTask<string?> GetCsrfStateAsync(CancellationToken cancellationToken = default)
        {
            lock (this.locker)
            {
                return ValueTask.FromResult(this.csrfState);
            }
        }

        public ValueTask ClearCsrfStateAsync(CancellationToken cancellationToken = default)
        {
            lock (this.locker)
            {
                this.csrfState = null;
            }

            return ValueTask.CompletedTask;
        }
    }
}
