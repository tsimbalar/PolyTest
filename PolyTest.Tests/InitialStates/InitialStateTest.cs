using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolyTest.Tests.InitialStates.Fluent;
using PolyTest.Tests.TestUtils;

namespace PolyTest.Tests.InitialStates
{
    [TestClass]
    public class InitialStateTest
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



    }

}
