// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Security.Cryptography;

namespace NHSDigital.ApiPlatform.Sdk.Brokers.Cryptographies
{
    internal sealed class CryptoBroker : ICryptoBroker
    {
        public string CreateUrlSafeState(int bytes = 32)
        {
            byte[] stateBytes = new byte[bytes];
            RandomNumberGenerator.Fill(stateBytes);

            return Convert.ToBase64String(stateBytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }
}
