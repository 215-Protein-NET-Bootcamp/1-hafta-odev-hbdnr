using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatikaOdev_1.Validation
{
    public interface IValidationResult
    {
        bool Success { get; }
        string ErrorMessage { get; }
    }
}
