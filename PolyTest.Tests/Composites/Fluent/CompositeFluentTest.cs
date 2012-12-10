using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolyTest.Tests.TestUtils;

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



    }

}
