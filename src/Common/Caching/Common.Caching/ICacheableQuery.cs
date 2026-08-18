using System;
using System.Collections.Generic;
using System.Text;
using Common.Messaging.Abstractions.Requests.Queries;

namespace Common.Caching
{
    public interface ICacheableQuery<TResult> : IQuery<TResult>
    {
        string CacheKey { get; }

        TimeSpan CacheDuration { get; }
    }
}
