using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatikaOdev_1.Validation
{
    public class ValidationResult : IValidationResult
    {
        public bool Success { get; }
        public string ErrorMessage { get; }

        public ValidationResult(bool success, string message ):this(success)
        {
            ErrorMessage = message;
        }

        public ValidationResult(bool success)
        {
            Success = success;
        }
    }
}
