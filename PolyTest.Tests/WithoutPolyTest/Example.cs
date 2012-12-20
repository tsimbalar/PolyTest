using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyTest.Tests.WithoutPolyTest
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

    public class ValidationResult
    {
        public ValidationResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public bool IsSuccess { get; private set; }
    }

    public class Input
    {
        public string Name { get; set; }

        public int Age { get; set; }
    }
}
