using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolyTest.Tests.TestUtils;
using System.Linq;

namespace PolyTest.Tests.Composites.Fluent
{
    [TestClass]
    public class CompositeFluentTest
    {
        [TestMethod]
        public void FluentTest()
        {
            // Arrange
            var sut = new TestAssets.FakeSut();

            TestTree.From("starting with 5", () => new TestAssets.DummyItem(5))
                 .Consider("add 2", d => { d.IntProperty = d.IntProperty + 2; })
                 .Consider("add 4", d => { d.IntProperty = d.IntProperty + 4; })
                 .Consider("add 1", d => { d.IntProperty++; },
                    opt => opt.IncludeSelf()
                        .Consider("add 13", d => { d.IntProperty = d.IntProperty + 13; })
                        .Consider("remove 3", d => { d.IntProperty = d.IntProperty - 3; })
                 )
                 .Consider("add 0", d => { },
                    opt => opt.IgnoreSelf()
                        .Consider("add 1", d => { d.IntProperty++; })
                        .Consider("remove 1", d => { d.IntProperty = d.IntProperty - 1; })
                        .Consider("add 3", d => { d.IntProperty += 3; },
                            opt2 => opt2
                                .Consider("add 4", d => { d.IntProperty += 4; })
                                .Consider("remove 2", d => { d.IntProperty -= 2; })
                        )
                 )
                 .Walk((state) =>
                            {
                                var init = state.Arrange();
                                sut.DoIt(init);
                                TestAssets.AssertIsNotFive(init.IntProperty, state.Description);
                            })
                 ;
        }

        [TestMethod]
        public void FluentTestWithResults()
        {
            // Arrange
            var sut = new TestAssets.FakeSut();
            var results =
            TestTree.From("starting with 5", () => new TestAssets.DummyItem(5))
                 .Consider("add 2", d => { d.IntProperty = d.IntProperty + 2; })
                 .Consider("add 4", d => { d.IntProperty = d.IntProperty + 4; })
                 .Consider("add 1", d => { d.IntProperty++; },
                    opt => opt.IncludeSelf()
                        .Consider("add 13", d => { d.IntProperty = d.IntProperty + 13; })
                        .Consider("remove 3", d => { d.IntProperty = d.IntProperty - 3; })
                 )
                 .Consider("add 0", d => { },
                    opt => opt.IgnoreSelf()
                        .Consider("add 1", d => { d.IntProperty++; })
                        .Consider("remove 1", d => { d.IntProperty = d.IntProperty - 1; })
                        .Consider("add 3", d => { d.IntProperty += 3; },
                            opt2 => opt2
                                .Consider("add 4", d => { d.IntProperty += 4; })
                                .Consider("remove 2", d => { d.IntProperty -= 2; })
                        )
                 )
                 .Walk((state) => state)
                 .Select(s =>
                             {
                                 var init = s.Arrange();
                                 var res = sut.DoIt(init);
                                 return String.Format("{0} -> {1}", s, res);
                             })
                 .ToList()
                 ;

            var result = string.Join("\n", results);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void FluentTestWithResultsTestResult()
        {
            // Arrange
            var sut = new TestAssets.FakeSut();
            TestTree.From("starting with 5", () => new TestAssets.DummyItem(5))
                 .Consider("add 2", d => { d.IntProperty = d.IntProperty + 2; })
                 .Consider("add 4", d => { d.IntProperty = d.IntProperty + 4; })
                 .Consider("add 1", d => { d.IntProperty++; },
                    opt => opt.IncludeSelf()
                        .Consider("add 13", d => { d.IntProperty = d.IntProperty + 13; })
                        .Consider("remove 3", d => { d.IntProperty = d.IntProperty - 3; })
                 )
                 .Consider("add 0", d => { },
                    opt => opt.IgnoreSelf()
                        .Consider("add 1", d => { d.IntProperty++; })
                        .Consider("remove 1", d => { d.IntProperty = d.IntProperty - 1; })
                        .Consider("add 3", d => { d.IntProperty += 3; },
                            opt2 => opt2
                                .Consider("add 4", d => { d.IntProperty += 4; })
                                .Consider("remove 2", d => { d.IntProperty -= 2; })
                                .Consider("remove 3", d => { d.IntProperty -= 3; })
                        )
                 )
                 .Walk(
                     act: d=> sut.DoIt(d),
                     assert: i=> TestAssets.AssertIsNotFive(i) 
                     )

                .AssertIsNotFailed()
                 ;

        }
    }

}
