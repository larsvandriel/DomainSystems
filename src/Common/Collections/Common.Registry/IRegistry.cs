using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Registry
{
    public interface IRegistry<TKey, TValue> : IReadOnlyRegistry<TKey, TValue> where TKey : notnull
    {
        void Register(TKey key, TValue value);
    }
}
