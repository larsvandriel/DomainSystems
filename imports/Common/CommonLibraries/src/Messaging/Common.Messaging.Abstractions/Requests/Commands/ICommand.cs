using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Messaging.Abstractions.Requests.Commands
{
    public interface ICommand<TResult> : IRequest<TResult>;
}
