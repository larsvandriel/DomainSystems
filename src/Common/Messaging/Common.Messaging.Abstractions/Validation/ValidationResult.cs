using System;
using System.Collections.Generic;
using System.Text;
using Common.Results.Problems;

namespace Common.Messaging.Abstractions.Validation
{
    public sealed class ValidationResult
    {
        private readonly ValidationErrors _errors = new();

        public bool IsValid => !_errors.Any;

        public void Add(string propertyname, string errorMessage)
        {
            _errors.Add(propertyname, errorMessage);
        }

        public IReadOnlyDictionary<string, string[]> ToDictionary()
        {
            return _errors.ToDictionary();
        }

        public void Merge(ValidationResult other)
        {
            ArgumentNullException.ThrowIfNull(other);

            foreach (var pair in other.ToDictionary())
            {
                foreach (var message in pair.Value)
                    _errors.Add(pair.Key, message);
            }
        }
    }
}
