using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolyTest.Tree;
using PolyTest.Examples.InputValidationExample.Models;
using Xunit;

namespace PolyTest.Examples.InputValidationExample
{
    /// <summary>
    /// Using PolyTest, with "raw" composite API
    /// </summary>
    public class WithPolyTest_Basic
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
            var startingWithValid = PolyTestTree.Root("Starting with valid input", () => MakeValidInput());
            startingWithValid.Add(PolyTestTree.Leaf(startingWithValid, Poly.Mutation<Input>("with null Name",  input=> input.Name = null)));
            startingWithValid.Add(PolyTestTree.Leaf(startingWithValid, Poly.Mutation<Input>("with empty Name", input => input.Name = String.Empty)));
            startingWithValid.Add(PolyTestTree.Leaf(startingWithValid, Poly.Mutation<Input>("with tab Name", input => input.Name = "\t")));
            startingWithValid.Add(PolyTestTree.Leaf(startingWithValid, Poly.Mutation<Input>("with space Name", input => input.Name = " ")));
            startingWithValid.Add(PolyTestTree.Leaf(startingWithValid, Poly.Mutation<Input>("with Age -1", input => input.Age = -1)));

            var sut = MakeSUT();
            startingWithValid.Walk((testcase) =>
            {
                // Arrange
                var initial = testcase.Arrange();
                // Act
                var actual = sut.Validate(initial);
                // Assert
                AssertIsInvalid(actual, testcase.Description);
            });
        }

        [Fact]
        public void Validate_with_invalidated_valid_Cheezburger_input_must_be_invalid()
        {
            // Arrange
            var startingWithValid = PolyTestTree.Root("Starting with valid input", () => MakeValidInput());

            var hasCheezburger = PolyTestTree.Composite(startingWithValid, new Mutation<Input>("with HasCheezburger true", d => { d.HasCheezburger = true; }));
            hasCheezburger.IncludeSelfInEnumeration = false; // we don't care about the case with HasCheezburger true and Cheezburger not specified
            hasCheezburger.Add(PolyTestTree.Leaf(hasCheezburger, Poly.Mutation<Input>("with no cheezburger", input=> {input.Cheezburger = null;})));
            startingWithValid.Add(hasCheezburger);


            var sut = MakeSUT();
            startingWithValid.Walk((testcase) =>
            {
                // Arrange
                var initial = testcase.Arrange();
                // Act
                var actual = sut.Validate(initial);
                // Assert
                AssertIsInvalid(actual, testcase.Description);
            });
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

        private static void AssertIsInvalid(ValidationResult validationResult, string description)
        {
            Assert.False(validationResult.IsSuccess, description);
        }

        private static void AssertIsValid(ValidationResult validationResult, string description)
        {
            Assert.True(validationResult.IsSuccess, description);
        }

        #endregion
    }
}
