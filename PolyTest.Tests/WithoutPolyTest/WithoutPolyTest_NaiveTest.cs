using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PolyTest.Tests.WithoutPolyTest
{
    /// <summary>
    /// Naive tests on the validation of Input.
    /// </summary>
    [TestClass]
    public class WithoutPolyTest_NaiveAttributeTest
    {

        [TestMethod]
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


        [TestMethod]
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

        [TestMethod]
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

        private Validator MakeSUT()
        {
            return new Validator();
        }

        private void AssertIsInvalid(ValidationResult validationResult, string description)
        {
            Assert.AreEqual(false, validationResult.IsSuccess, description);
        }

        #endregion
    }

}
