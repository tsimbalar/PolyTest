using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolyTest.Examples.InputValidationExample.Models;
using PolyTest;
using PolyTest.Fluent;
using Xunit;

namespace PolyTest.Examples.InputValidationExample
{
    public class WithPolyTest_Composite_Fluent
    {
        [Fact]
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


        [Fact]
        public void Validate_with_invalidated_valid_input_must_be_invalid()
        {
            // Arrange
            var sut = MakeSUT();

            Poly.Factory.From("Starting with valid input", () => MakeValidInput())
                    .Consider("with null name", input => input.Name = null)
                    .Consider("with empty name", input => input.Name = String.Empty)
                    .Consider("with tab Name", input => input.Name = "\t")
                    .Consider("with space Name", input => input.Name = " ")
                    .Consider("with Age -1", input => input.Age = -1)
                    .Walk(
                        act: input => sut.Validate(input),
                        assert: actual => AssertIsInvalid(actual)
                )
                .AssertAllPassed();
        }

        [Fact]
        public void Validate_with_invalidated_valid_Cheezburger_input_must_be_invalid()
        {
            // Arrange
            var sut = MakeSUT();

            Poly.Factory.From("Starting with valid input", () => MakeValidInput())
                .Consider("with HasCheezburger true", input => input.HasCheezburger = true,
                    fromThere => fromThere.IgnoreSelf("we don't care about the case with HasCheezburger true and Cheezburger not specified")
                        .Consider("with no cheezburger", input => input.Cheezburger = null)
 
                )
                .Walk(
                    act: input => sut.Validate(input),
                    assert: actual => AssertIsInvalid(actual)
                )
                .AssertAllPassed();
        }

        #region Test Helper Methods

        private static Input MakeValidInput()
        {
            return new Input
            {
                Age = 4,
                Name = "a valid name",
                HasCheezburger = true,
                Cheezburger = new Cheezburger()
            };
        }

        private static Validator MakeSUT()
        {
            return new Validator();
        }

        private static void AssertIsInvalid(ValidationResult validationResult, string description = "")
        {
            Assert.False(validationResult.IsSuccess, description);
        }

        private static void AssertIsValid(ValidationResult validationResult, string description = "")
        {
            Assert.True(validationResult.IsSuccess, description);
        }

        #endregion
    }
}
