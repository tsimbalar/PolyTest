using System;
using PolyTest.Examples.InputValidationExample.Models;
using Xunit;

namespace PolyTest.Examples.InputValidationExample
{
    /// <summary>
    /// Naive tests on the validation of Input.
    /// </summary>
    public class WithoutPolyTest_NaiveAttributeTest
    {

        [Fact]
        public void Validate_with_input_with_null_Name_must_be_invalid()
        {
            // Arrange
            var invalidInput = new Input { Name = null };
            var sut = MakeSUT();

            // Act
            var validationResult = sut.Validate(invalidInput);

            // Assert
            AssertIsInvalid(validationResult, "input with null Name should be invalid");
        }


        [Fact]
        public void Validate_with_input_with_empty_Name_must_be_invalid()
        {
            // Arrange
            var invalidInput = new Input { Name = String.Empty };
            var sut = MakeSUT();

            // Act
            var validationResult = sut.Validate(invalidInput);

            // Assert
            AssertIsInvalid(validationResult, "input with empty Name should be invalid");
        }

        [Fact]
        public void Validate_with_input_with_Age_minus_one_must_be_invalid()
        {
            // Arrange
            var invalidInput = new Input { Name = "someName", Age = -1 };
            var sut = MakeSUT();

            // Act
            var validationResult = sut.Validate(invalidInput);

            // Assert
            AssertIsInvalid(validationResult, "input with Age -1 should be invalid");
        }


        #region Test Helper Methods

        private static Validator MakeSUT()
        {
            return new Validator();
        }

        private static void AssertIsInvalid(ValidationResult validationResult, string description)
        {
            Assert.False(validationResult.IsSuccess, description);
        }

        #endregion
    }

}
