using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolyTest.Tests.InitialStates.Fluent;
using PolyTest.Tests.TestUtils;

namespace PolyTest.Tests.InitialStates
{
    [TestClass]
    public class BlahTest
    {

        [TestMethod]
        public void FirstTest()
        {
            // Arrange
            var sut = new TestAssets.FakeSut();

            var initialCondition = new StartingPoint<TestAssets.DummyItem>("Starting with IntProperty = 5", () => new TestAssets.DummyItem(5));

            IInitialStateCollection<TestAssets.DummyItem> testCases = new InitialStateCollection<TestAssets.DummyItem>();
            testCases.Add(new SequentialMutations<TestAssets.DummyItem>(initialCondition, new Mutation<TestAssets.DummyItem>("setting it to 4", d => { d.IntProperty = 4; })));
            testCases.Add(new SequentialMutations<TestAssets.DummyItem>(initialCondition, new Mutation<TestAssets.DummyItem>("setting it to 3", d => { d.IntProperty = 3; })));

            var adding2 = new SequentialMutations<TestAssets.DummyItem>(initialCondition, new Mutation<TestAssets.DummyItem>("adding 2", d => { d.IntProperty = d.IntProperty + 2; }));
            testCases.Add(new SequentialMutations<TestAssets.DummyItem>(adding2, new Mutation<TestAssets.DummyItem>("removing 1", d => { d.IntProperty = d.IntProperty - 1; })));
            testCases.Add(new SequentialMutations<TestAssets.DummyItem>(adding2, new Mutation<TestAssets.DummyItem>("removing 3", d => { d.IntProperty = d.IntProperty - 3; })));



            var results = testCases.Execute(it => sut.DoIt(it));


            results.ForEach((str, val) => TestAssets.AssertIsNotFive(val, str));


            // Act

            // Assert

        }

        [TestMethod]
        public void FluentFlatTest()
        {
            // Arrange
            var sut = new TestAssets.FakeSut();
            "Starting with IntProperty = 5".AsStartingPoint(() => new TestAssets.DummyItem(5))
                .Arrange("setting it to 4", d => { d.IntProperty = 4; })
                .Arrange("setting it to 3", d => { d.IntProperty = 3; })
            .Act(it => sut.DoIt(it))
            .Assert((str, val) => TestAssets.AssertIsNotFive(val, str));

            // Act

            // Assert

        }

        [TestMethod]
        public void FluentNestedTest()
        {
            // Arrange
            var sut = new TestAssets.FakeSut();
            "Starting with IntProperty = 5".AsStartingPoint(() => new TestAssets.DummyItem(5))
                .Arrange("setting it to 4", d => { d.IntProperty = 4; },
                    andThen => andThen.IgnoringRoot()
                        .With(new Mutation<TestAssets.DummyItem>("setting bool to false", d => { d.BoolProperty = false; }))
                        .With(new Mutation<TestAssets.DummyItem>("setting bool to true", d => { d.BoolProperty = true; }))
                )
                .Arrange("setting it to 3", d => { d.IntProperty = 3; })
                // Act
            .Act(it =>
                     {
                         sut.HasIt(it);
                         return it;
                     })
                // Assert
            .Assert((str, val) => Assert.AreEqual(true, val.BoolProperty, str));
        }


    }

}
