using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatikaOdev_1.Validation
{
    public class ValidationHelper
    {
        public static IValidationResult Run(params IValidationResult[] validations)
        {
            foreach (var validation in validations)
            {
                if (validation.Success == false)
                {
                    return validation;
                }
            }
            return null;
        }
    }
}
