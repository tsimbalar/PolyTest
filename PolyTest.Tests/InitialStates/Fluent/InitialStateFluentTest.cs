using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolyTest.Tests.TestUtils;

namespace PolyTest.Tests.InitialStates.Fluent
{
    [TestClass]
    public class InitialStateFluentTest
    {

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
            "Starting with IntProperty = 5".AsStartingPoint(() => new TestAssets.DummyItem(5, true))
                .Arrange("setting it to 4", d => { d.IntProperty = 4; },
                    andThen => andThen.IgnoringRoot()
                        //.WithChange(new Mutation<TestAssets.DummyItem>("setting bool to false", d => { d.BoolProperty = false; }))
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
