using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PolyTest.Tests
{
    public class PolyTest
    {

        [Fact]
        public void Poly_Create_returns_a_DefaultPolyTestFactory()
        {
            // Arrange

            // Act
            var actual = Poly.Create;

            // Assert
            Assert.IsType<DefaultPolyTestFactory>(actual);
        }

        [Fact]
        public void Poly_Create_always_returns_same_Factory()
        {
            // Arrange
            var firstObtained = Poly.Create;
            // Act
            var actual = Poly.Create;

            // Assert
            Assert.Same(firstObtained, actual);
        }

        [Fact]
        public void Poly_Runner_returns_a_DefaultPolyTestRunner()
        {
            // Arrange

            // Act
            var actual = Poly.Runner;

            // Assert
            Assert.IsType<DefaultPolyTestRunner>(actual);
        }

        [Fact]
        public void Poly_Runner_always_returns_same_TestRunner()
        {
            // Arrange
            var firstObtained = Poly.Runner;
            // Act
            var actual = Poly.Runner;

            // Assert
            Assert.Same(firstObtained, actual);
        }
    }
}
