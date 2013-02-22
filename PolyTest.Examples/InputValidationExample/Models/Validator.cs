using System;

namespace PolyTest.Examples.InputValidationExample.Models
{
    public class Validator
    {
        public ValidationResult Validate(Input input)
        {
            var isValid = true;
            if (String.IsNullOrWhiteSpace(input.Name)) isValid = false;
            if (input.Age == -1) isValid = false;
            if (input.HasCheezburger)
            {
                if (input.Cheezburger == null) isValid = false;
            }
            return new ValidationResult(isValid);
        }
    }
}