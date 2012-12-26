using PolyTest.StartFrom.Fluent;
using PolyTest.Tests.TestUtils;
using Xunit;

namespace PolyTest.Tests.InitialStates.Fluent
{
    public class InitialStateFluentTest
    {

        [Fact]
        public void FluentFlatTest()
        {
            // Arrange
            var sut = new FakeSut();
            "Starting with IntProperty = 5".AsStartingPoint(() => new DummyItem(5))
                .Arrange("setting it to 4", d => { d.IntProperty = 4; })
                .Arrange("setting it to 3", d => { d.IntProperty = 3; })
            .Act(it => sut.DoIt(it))
            .Assert((str, val) => DummyAssert.AssertIsNotFive(val, str));

            // Act

            // Assert

        }

        [Fact]
        public void FluentNestedTest()
        {
            // Arrange
            var sut = new FakeSut();
            "Starting with IntProperty = 5".AsStartingPoint(() => new DummyItem(5, true))
                .Arrange("setting it to 4", d => { d.IntProperty = 4; },
                    andThen => andThen.IgnoringRoot()
                        //.WithChange(new Mutation<DummyItem>("setting bool to false", d => { d.BoolProperty = false; }))
                        .With(new Mutation<DummyItem>("setting bool to true", d => { d.BoolProperty = true; }))
                )
                .Arrange("setting it to 3", d => { d.IntProperty = 3; })
                // Act
            .Act(it =>
            {
                sut.HasIt(it);
                return it;
            })
                // Assert
            .Assert((str, val) => Assert.True(val.BoolProperty, str));
        }
    }
}
