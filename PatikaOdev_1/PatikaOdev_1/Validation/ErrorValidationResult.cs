using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatikaOdev_1.Validation
{
    public class ErrorValidationResult : ValidationResult
    {
        public ErrorValidationResult(string message):base(false,message)
        {

        }
        public ErrorValidationResult():base(false)
        {

        }
    }
}
