﻿using System;
using PolyTest.Tests.TestUtils;
using System.Linq;
using PolyTest.Tree.Fluent;
using PolyTest.Tree.Fluent.Magic;
using Xunit;

namespace PolyTest.Tests.Composites.Fluent
{
    public class CompositeFluentTest
    {
        [Fact]
        public void FluentTest()
        {
            // Arrange
            var sut = new FakeSut();

            TestTree.From("starting with 5", () => new DummyItem(5))
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
                                DummyAssert.AssertIsNotFive(init.IntProperty, state.Description);
                            })
                 ;
        }


        [Fact]
        public void FluentTestWithResults()
        {
            // Arrange
            var sut = new FakeSut();
            var results =
            TestTree.From("starting with 5", () => new DummyItem(5))
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
            Assert.Equal("", result);
        }

        [Fact]
        public void FluentTestWithResultsTestResult()
        {
            // Arrange
            var sut = new FakeSut();
            TestTree.From("starting with 5", () => new DummyItem(5))
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
                                //.Consider("remove 3", d => { d.IntProperty -= 3; })
                        )
                 )
                 .Walk(
                     act: d => sut.DoIt(d),
                     assert: i => DummyAssert.AssertIsNotFive(i)
                     )

                .AssertIsNotFailed()
                 ;

        }



        [Fact]
        public void FluentTestWithReflection()
        {
            // Arrange
            var sut = new FakeSut();

            TestTree.From("starting with 5", () => new DummyItem(5))
                    .With(d => d.IntProperty, 6)
                    .With(d => d.IntProperty, 3,
                        opt => opt.IncludeSelf()
                            .Consider("add 13", d => { d.IntProperty = d.IntProperty + 13; })
                            .Consider("remove 3", d => { d.IntProperty = d.IntProperty - 3; })
                            .With(d => d.BoolProperty, false)
                )
                .Walk(act: d => sut.DoIt(d), assert: a =>
                                                         {
                                                             DummyAssert.AssertIsNotFive(a);
                                                         })
                .AssertIsNotFailed();

        }
    }

}
