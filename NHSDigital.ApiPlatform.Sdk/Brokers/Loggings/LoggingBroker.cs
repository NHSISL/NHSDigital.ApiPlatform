// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSDigital.ApiPlatform.Sdk.Brokers.Loggings
{
    internal sealed class LoggingBroker : ILoggingBroker
    {
        private readonly ILogger<LoggingBroker> logger;

        public LoggingBroker(ILogger<LoggingBroker> logger) =>
            this.logger = logger;

        public ValueTask LogErrorAsync(Exception exception)
        {
            this.logger.LogError(exception, exception.Message);

            return ValueTask.CompletedTask;
        }

        public ValueTask LogCriticalAsync(Exception exception)
        {
            this.logger.LogCritical(exception, exception.Message);

            return ValueTask.CompletedTask;
        }
    }
}
