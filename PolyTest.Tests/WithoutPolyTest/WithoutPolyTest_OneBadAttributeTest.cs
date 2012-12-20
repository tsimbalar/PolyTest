using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PolyTest.Tests.WithoutPolyTest
{
    /// <summary>
    /// Once we apply the One Bad Attribute pattern
    /// </summary>
    [TestClass]
    public class WithoutPolyTest_OneBadAttributeTest
    {
        [TestMethod]
        public void Validate_with_valid_input_must_be_valid()
        {
            // Arrange
            var input = MakeValidInput();
            var sut = MakeSUT();

            // Act
            var validationResult = sut.Validate(input);

            // Assert
            AssertIsValid(validationResult, "valid case should be valid (duh!)");
        }

        [TestMethod]
        public void Validate_with_input_with_null_Name_must_be_invalid()
        {
            // Arrange
            var input = MakeValidInput();
            input.Name = null;
            var sut = MakeSUT();

            // Act
            var validationResult = sut.Validate(input);

            // Assert
            AssertIsInvalid(validationResult, "input with null Name should be invalid");
        }

        [TestMethod]
        public void Validate_with_input_with_empty_Name_must_be_invalid()
        {
            // Arrange
            var input = MakeValidInput();
            input.Name = String.Empty;
            var sut = MakeSUT();

            // Act
            var validationResult = sut.Validate(input);

            // Assert
            AssertIsInvalid(validationResult, "input with empty Name should be invalid");
        }

        [TestMethod]
        public void Validate_with_input_with_Age_minus_one_must_be_invalid()
        {
            // Arrange
            var input = MakeValidInput();
            input.Age = -1;
            var sut = new Validator();

            // Act
            var validationResult = sut.Validate(input);

            // Assert
            AssertIsInvalid(validationResult, "input with Age -1 should be invalid");
        }

        #region Test Helper Methods

        private Input MakeValidInput()
        {
            return new Input
            {
                Age = 4,
                Name = "a valid name"
            };
        }

        private Validator MakeSUT()
        {
            return new Validator();
        }

        private void AssertIsInvalid(ValidationResult validationResult, string description)
        {
            Assert.AreEqual(false, validationResult.IsSuccess, description);
        }

        private void AssertIsValid(ValidationResult validationResult, string description)
        {
            Assert.AreEqual(true, validationResult.IsSuccess, description);
        }

        #endregion

        
    }

}
