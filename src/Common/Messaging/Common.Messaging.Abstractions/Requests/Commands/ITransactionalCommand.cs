using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Messaging.Abstractions.Requests.Commands
{
    public interface ITransactionalCommand<TResult> : ICommand<TResult>;
}
