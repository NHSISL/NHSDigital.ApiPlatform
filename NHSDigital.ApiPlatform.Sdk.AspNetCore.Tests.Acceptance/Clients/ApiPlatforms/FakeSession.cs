// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NHSDigital.ApiPlatform.Sdk.AspNetCore.Tests.Acceptance.Clients.ApiPlatforms
{
    internal sealed class FakeSession : ISession
    {
        private readonly Dictionary<string, byte[]> store = new Dictionary<string, byte[]>();

        public bool IsAvailable => true;
        public string Id => "acceptance-session";
        public IEnumerable<string> Keys => this.store.Keys;

        public void Clear() => this.store.Clear();

        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void Remove(string key) => this.store.Remove(key);

        public void Set(string key, byte[] value) => this.store[key] = value;

        public bool TryGetValue(string key, out byte[] value) => this.store.TryGetValue(key, out value);
    }
}
