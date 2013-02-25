﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolyTest.Examples.InputValidationExample.Models;
using Xunit;

namespace PolyTest.Examples.InputValidationExample
{
    /// <summary>
    /// Using PolyTest, with "raw" composite API
    /// </summary>
    public class WithPolyTest_Composite_Basic
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
            var validRoot = Poly.Root("Starting with valid input", () => MakeValidInput());

            var nullNameMutation = Poly.Mutation<Input>("with null name", input => input.Name = null);
            var nullNameLeaf = Poly.Leaf(validRoot, nullNameMutation);
            validRoot.Add(nullNameLeaf);

            var emptyNameMutation = Poly.Mutation<Input>("with empty name", input => input.Name = String.Empty);
            var emptyNameLeaf = Poly.Leaf(validRoot, emptyNameMutation);
            validRoot.Add(emptyNameLeaf);

            var tabNameMutation = Poly.Mutation<Input>("with tab Name", input => input.Name = "\t");
            var tabNameLeaf = Poly.Leaf(validRoot, tabNameMutation);
            validRoot.Add(tabNameLeaf);

            var spaceNameMutation = Poly.Mutation<Input>("with space Name", input => input.Name = " ");
            var spaceNameLeaf = Poly.Leaf(validRoot, spaceNameMutation);
            validRoot.Add(spaceNameLeaf);

            var minusOneAgeMutation = Poly.Mutation<Input>("with Age -1", input => input.Age = -1);
            var minusOneAgeLeaf = Poly.Leaf(validRoot, minusOneAgeMutation);
            validRoot.Add(minusOneAgeLeaf);

            var sut = MakeSUT();
            // walk through the tree, yielding every possible path
            validRoot.Walk((testcase) =>
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
            var validRoot = Poly.Root("Starting with valid input", () => MakeValidInput());

            var hasCheezburgerMutation = Poly.Mutation<Input>("with HasCheezburger true", input => input.HasCheezburger = true);
            var intermediaryNodeWithHasCheezburgerTrue = Poly.Composite(validRoot, hasCheezburgerMutation);
            intermediaryNodeWithHasCheezburgerTrue.IncludeSelfInEnumeration = false; //we don't care about the case with HasCheezburger true and Cheezburger not specified

            var cheezburgerNullMutation = Poly.Mutation<Input>("with no cheezburger", input => input.Cheezburger = null);
            var cheezburgerNullLeaf = Poly.Leaf(intermediaryNodeWithHasCheezburgerTrue, cheezburgerNullMutation);
            intermediaryNodeWithHasCheezburgerTrue.Add(cheezburgerNullLeaf);
            
            validRoot.Add(intermediaryNodeWithHasCheezburgerTrue);

            var sut = MakeSUT();
            validRoot.Walk((testcase) =>
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
