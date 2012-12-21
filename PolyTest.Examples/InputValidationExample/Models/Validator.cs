using System;

namespace PolyTest.Examples.InputValidationExample.Models
{
    public class Validator
    {
        public ValidationResult Validate(Input input)
        {
            var isValid = true;
            if (String.IsNullOrEmpty(input.Name)) isValid = false;
            if (input.Age == -1) isValid = false;
            return new ValidationResult(isValid);
        }
    }
}