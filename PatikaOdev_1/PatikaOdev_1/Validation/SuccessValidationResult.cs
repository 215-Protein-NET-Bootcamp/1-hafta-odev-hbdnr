using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatikaOdev_1.Validation
{
    public class SuccessValidationResult : ValidationResult
    {
        public SuccessValidationResult(string message):base(true,message)
        {

        }
        public SuccessValidationResult():base(true)
        {

        }
    }
}
