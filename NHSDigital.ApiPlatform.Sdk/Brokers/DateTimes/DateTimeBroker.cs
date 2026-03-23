// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------
using System;

namespace NHSDigital.ApiPlatform.Sdk.Brokers.DateTimes
{
    internal class DateTimeBroker : IDateTimeBroker
    {
        public DateTimeOffset GetCurrentDateTimeOffset() => DateTimeOffset.UtcNow;
    }
}
