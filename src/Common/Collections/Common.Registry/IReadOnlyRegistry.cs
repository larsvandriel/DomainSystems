using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Registry
{
    public interface IReadOnlyRegistry<TKey, TValue> where TKey : notnull
    {
        bool TryGet(TKey key, out TValue? value);

        TValue GetRequired(TKey key);
    }
}
