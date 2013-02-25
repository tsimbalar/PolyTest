using System.Linq;
using PolyTest.Examples.CarExample.Models;
using PolyTest.Fluent;
using PolyTest.Fluent.Magic;
using Xunit;

namespace PolyTest.Examples.CarExample
{
    public class WithPolyTest_TestTree_Fluent_MagicTest
    {

        [Fact]
        public void ValidCase()
        {

            var sut = new CarValidator();
            Assert.True(sut.Validate(MakeValidCar()), "valid case should be valid (duh)");

        }

        [Fact]
        public void ValidationExampleTest()
        {
            var sut = new CarValidator();
            TestTree.From("A car with all the proper attributes", 
                            () => MakeValidCar())
                    .With(c => c.NumberOfDoors, 0)
                    .With(c => c.NumberOfDoors, -1)
                    .WithNull(c => c.BrandName)
                    .WithNullOrWhitespace(c => c.ModelName)
                    .WithFalse(c => c.NeedsEngine,
                        fromThere => fromThere.IgnoreSelf("ignore case with only 'NeedsEngine = false'")
                            .With(c => c.Engine, new Engine())
                    )
                    .WithTrue(c => c.NeedsEngine,
                        fromHere => fromHere.IgnoreSelf("ignore case with only 'NeedsEngine = true'")
                            .WithNull(c => c.Engine)
                    )
                    .Walk(
                        act: o => sut.Validate(o),
                        assert: actual => AssertIsInvalid(actual))
                    .AssertAllPassed();

        }

        [Fact]
        public void ValidationWheelsExampleTest()
        {
            var sut = new CarValidator();
            TestTree.From("Valid Car", 
                            () => MakeValidCar())
                    .Consider("No wheels !",
                                c => c.Wheels.Clear())
                    .Consider("1 more wheel !",
                                c => c.Wheels.Add(new Wheel(c,5)))
                    .Walk(
                        act: o => sut.Validate(o),
                        assert: actual => AssertIsInvalid(actual))
                    .AssertAllPassed();

        }

        private static void AssertIsInvalid(bool validationResult)
        {
            Assert.False(validationResult, "Should be false to be considered invalid");
        }

        private Car MakeValidCar()
        {
            var c = new Car();
            c.BrandName = "Renault";
            c.ModelName = "Megane";
            c.NumberOfDoors = 3;
            // add 4 wheels
            Enumerable.Range(0, 4).ToList()
                .ForEach(i => c.Wheels.Add(new Wheel(c, i)));
            c.Engine = new DieselEngine();
            return c;
        }
    }
}
