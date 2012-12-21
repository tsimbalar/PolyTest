using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolyTest.Tests.TestUtils;
using PolyTest.Tree;

namespace PolyTest.Tests.Composites
{
    [TestClass]
    public class CompositeTest
    {

        [TestMethod]
        public void Can_use_Composite()
        {
            // Arrange
            var sut = new TestAssets.FakeSut();

            var rootStartingAt5 = new TestRoot<TestAssets.DummyItem>("Starting with IntProperty = 5", () => new TestAssets.DummyItem(5));

            rootStartingAt5.Add(new TestLeaf<TestAssets.DummyItem>(rootStartingAt5, new Mutation<TestAssets.DummyItem>("setting it to 4", d => { d.IntProperty = 4; })));
            rootStartingAt5.Add(new TestLeaf<TestAssets.DummyItem>(rootStartingAt5, new Mutation<TestAssets.DummyItem>("setting it to 3", d => { d.IntProperty = 3; })));

            var adding2 = new TestComposite<TestAssets.DummyItem>(rootStartingAt5, new Mutation<TestAssets.DummyItem>("adding 2", d => { d.IntProperty = d.IntProperty + 2; }), includeInEnumeration: true);
            adding2.Add(new TestLeaf<TestAssets.DummyItem>(adding2, new Mutation<TestAssets.DummyItem>("removing 1", d => { d.IntProperty = d.IntProperty - 1; })));
            adding2.Add(new TestLeaf<TestAssets.DummyItem>(adding2, new Mutation<TestAssets.DummyItem>("removing 3", d => { d.IntProperty = d.IntProperty - 3; })));
            rootStartingAt5.Add(adding2);

            var adding3 = new TestComposite<TestAssets.DummyItem>(rootStartingAt5, new Mutation<TestAssets.DummyItem>( "adding 3", d => { d.IntProperty = d.IntProperty + 3; }), includeInEnumeration: true);
            adding2.Add(new TestLeaf<TestAssets.DummyItem>(adding3, new Mutation<TestAssets.DummyItem>("removing 2", d => { d.IntProperty = d.IntProperty - 2; })));
            adding2.Add(new TestLeaf<TestAssets.DummyItem>(adding3, new Mutation<TestAssets.DummyItem>("removing 4", d => { d.IntProperty = d.IntProperty - 4; })));

            rootStartingAt5.Add(adding3);

            //Assert.AreEqual("", String.Join("\n", rootStartingAt5.Enumerate().ToList().Select(t => t.Description)));

            rootStartingAt5.Walk((start) =>
            {
                var initial = start.Arrange();
                var actual = sut.DoIt(initial);
                TestAssets.AssertIsNotFive(actual, start.Description);
            });


            // Act

            // Assert
        }
    }
}
