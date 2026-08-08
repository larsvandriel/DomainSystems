using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Messaging.Abstractions.Requests.Queries
{
    public interface IQuery<TResult> : IRequest<TResult>;
}
