using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PolyTest.Tests.WithoutPolyTest
{
    [TestClass]
    public class WithoutPolyTestTest
    {
        [TestMethod]
        public void Validate_with_input_with_null_Name_must_return_false()
        {
            // Arrange
            var invalidInput = new Input { Name = null };
            var sut = new Validator();

            // Act
            var validationResult = sut.Validate(invalidInput);

            // Assert
            AssertIsInvalid(validationResult, "input with null Name should be invalid");
        }

        [TestMethod]
        public void Validate_with_input_with_empty_Name_must_return_false()
        {
            // Arrange
            var invalidInput = new Input { Name = String.Empty };
            var sut = new Validator();

            // Act
            var validationResult = sut.Validate(invalidInput);

            // Assert
            AssertIsInvalid(validationResult, "input with empty Name should be invalid");
        }

        [TestMethod]
        public void Validate_with_input_with_Age_minus_one_must_return_false()
        {
            // Arrange
            var invalidInput = new Input { Name = "someName", Age = -1 };
            var sut = new Validator();

            // Act
            var validationResult = sut.Validate(invalidInput);

            // Assert
            AssertIsInvalid(validationResult, "input with Age -1 should be invalid");
        }




        private void AssertIsInvalid(ValidationResult validationResult, string description)
        {
            Assert.AreEqual(true, validationResult.IsSuccess, description);
        }
    }

    public class Validator
    {
        public ValidationResult Validate(Input input)
        {
            return new ValidationResult();
        }
    }

    public class ValidationResult
    {
        public bool IsSuccess { get; set; }
    }

    public class Input
    {
        public string Name { get; set; }

        public int Age { get; set; }
    }
}
