using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolyTest.Tests.TestUtils;

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

            rootStartingAt5.Add(new TestLeaf<TestAssets.DummyItem>(rootStartingAt5, "setting it to 4", d => { d.IntProperty = 4; }));
            rootStartingAt5.Add(new TestLeaf<TestAssets.DummyItem>(rootStartingAt5, "setting it to 3", d => { d.IntProperty = 3; }));

            var adding2 = new TestComposite<TestAssets.DummyItem>(rootStartingAt5, "adding 2", d => { d.IntProperty = d.IntProperty + 2; }, includeInEnumeration: true);
            adding2.Add(new TestLeaf<TestAssets.DummyItem>(adding2, "removing 1", d => { d.IntProperty = d.IntProperty - 1; }));
            adding2.Add(new TestLeaf<TestAssets.DummyItem>(adding2, "removing 3", d => { d.IntProperty = d.IntProperty - 3; }));
            rootStartingAt5.Add(adding2);

            var adding3 = new TestComposite<TestAssets.DummyItem>(rootStartingAt5, "adding 3", d => { d.IntProperty = d.IntProperty + 3; }, includeInEnumeration: true);
            adding2.Add(new TestLeaf<TestAssets.DummyItem>(adding3, "removing 2", d => { d.IntProperty = d.IntProperty - 2; }));
            adding2.Add(new TestLeaf<TestAssets.DummyItem>(adding3, "removing 4", d => { d.IntProperty = d.IntProperty - 4; }));

            rootStartingAt5.Add(adding3);

            //Assert.AreEqual("", String.Join("\n", rootStartingAt5.Enumerate().ToList().Select(t => t.Description)));

            rootStartingAt5.Walk((testCaseDescription, arrange) =>
            {
                var initial = arrange();
                var actual = sut.DoIt(initial);
                TestAssets.AssertIsNotFive(actual, testCaseDescription);
            });


            // Act

            // Assert
        }
    }
}
