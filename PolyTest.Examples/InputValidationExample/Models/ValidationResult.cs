namespace PolyTest.Examples.InputValidationExample.Models
{
    public class ValidationResult
    {
        public ValidationResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public bool IsSuccess { get; private set; }
    }
}
