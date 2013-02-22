using PolyTest.Tests.TestUtils;
using PolyTest.Tree;
using Xunit;

namespace PolyTest.Tests.Composites
{
    public class CompositeTest
    {

        [Fact]
        public void Can_use_Composite()
        {
            // Arrange
            var sut = new FakeSut();

            var rootStartingAt5 = new TestRoot<DummyItem>("Starting with IntProperty = 5", () => new DummyItem(5));

            rootStartingAt5.Add(new TestLeaf<DummyItem>(rootStartingAt5, new Mutation<DummyItem>("setting it to 4", d => { d.IntProperty = 4; })));
            rootStartingAt5.Add(new TestLeaf<DummyItem>(rootStartingAt5, new Mutation<DummyItem>("setting it to 3", d => { d.IntProperty = 3; })));

            var adding2 = new TestComposite<DummyItem>(rootStartingAt5, new Mutation<DummyItem>("adding 2", d => { d.IntProperty = d.IntProperty + 2; }));
            adding2.Add(new TestLeaf<DummyItem>(adding2, new Mutation<DummyItem>("removing 1", d => { d.IntProperty = d.IntProperty - 1; })));
            adding2.Add(new TestLeaf<DummyItem>(adding2, new Mutation<DummyItem>("removing 3", d => { d.IntProperty = d.IntProperty - 3; })));
            rootStartingAt5.Add(adding2);

            var adding3 = new TestComposite<DummyItem>(rootStartingAt5, new Mutation<DummyItem>( "adding 3", d => { d.IntProperty = d.IntProperty + 3; }));
            adding2.Add(new TestLeaf<DummyItem>(adding3, new Mutation<DummyItem>("removing 2", d => { d.IntProperty = d.IntProperty - 2; })));
            adding2.Add(new TestLeaf<DummyItem>(adding3, new Mutation<DummyItem>("removing 4", d => { d.IntProperty = d.IntProperty - 4; })));

            rootStartingAt5.Add(adding3);

            //Assert.AreEqual("", String.Join("\n", rootStartingAt5.Enumerate().ToList().Select(t => t.Description)));

            rootStartingAt5.Walk((start) =>
            {
                var initial = start.Arrange();
                var actual = sut.DoIt(initial);
                DummyAssert.AssertIsNotFive(actual, start.Description);
            });


            // Act

            // Assert
        }
    }
}
