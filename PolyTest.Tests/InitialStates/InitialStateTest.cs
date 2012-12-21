using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolyTest.StartFrom;
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
            var sut = new FakeSut();

            var initialCondition = new StartingPoint<DummyItem>("Starting with IntProperty = 5", () => new DummyItem(5));

            IInitialStateCollection<DummyItem> testCases = new InitialStateCollection<DummyItem>();
            testCases.Add(new SequentialMutations<DummyItem>(initialCondition, new Mutation<DummyItem>("setting it to 4", d => { d.IntProperty = 4; })));
            testCases.Add(new SequentialMutations<DummyItem>(initialCondition, new Mutation<DummyItem>("setting it to 3", d => { d.IntProperty = 3; })));

            var adding2 = new SequentialMutations<DummyItem>(initialCondition, new Mutation<DummyItem>("adding 2", d => { d.IntProperty = d.IntProperty + 2; }));
            testCases.Add(new SequentialMutations<DummyItem>(adding2, new Mutation<DummyItem>("removing 1", d => { d.IntProperty = d.IntProperty - 1; })));
            testCases.Add(new SequentialMutations<DummyItem>(adding2, new Mutation<DummyItem>("removing 3", d => { d.IntProperty = d.IntProperty - 3; })));



            var results = testCases.Execute(it => sut.DoIt(it));


            results.ForEach((str, val) => DummyAssert.AssertIsNotFive(val, str));


            // Act

            // Assert

        }



    }

}
