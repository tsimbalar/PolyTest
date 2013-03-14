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
        public void Poly_Create_returns_a_PolyDefaultTestFactory()
        {
            // Arrange

            // Act
            var actual = Poly.Create;

            // Assert
            Assert.IsType<PolyDefaultTestFactory>(actual);
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
    }
}
