using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PolyTest.Tests.Fluent
{
    [TestClass]
    public class UnitTest1
    {

        //[TestMethod]
        //public void FirstFluentTest()
        //{
        //    // Arrange
        //    var sut = new FakeSut();

        //    var testSuite = Poly.Test("Starting with IntProperty = 5", () => new DummyItem(5))
        //                        .WithChange("setting int to 4", it => { it.IntProperty = 4; })

        //                        .When(it => sut.DoIt(it))
        //                        .Then((i, desc) => AssertIsNotFive(i, desc));
        //    testSuite.Launch();

        //    // Act

        //    // Assert

        //}




        //public static class Poly
        //{
        //    public static IInitialState<T> Test<T>(string initialConditionDescription, Func<T> initial)
        //    {
        //        return new MultiTest<T>(new InitialConditionFactory<T>(initialConditionDescription, initial));
        //    }
        //}

        //public interface IInitialConditionFactory<T>
        //{
        //    IInitialization<T> CreateCondition(IMutation<T> mutation);
        //}

        //public class InitialConditionFactory<T> : IInitialConditionFactory<T>
        //{
        //    private readonly string _initialStateDescription;
        //    private readonly Func<T> _initialStateMethod;

        //    public InitialConditionFactory(string initialStateDescription, Func<T> initialStateMethod)
        //    {
        //        _initialStateDescription = initialStateDescription;
        //        _initialStateMethod = initialStateMethod;
        //    }

        //    public InitialConditionFactory(IInitialization<T> initialCondition)
        //        :this(initialCondition.Description, initialCondition.Prepare)
        //    {

        //    }

        //    public IInitialization<T> CreateCondition(IMutation<T> mutation)
        //    {
        //        return new StartingPoint<T>(_initialStateDescription +  " -> " + mutation.Description, () =>
        //                                           {
        //                                               var start = _initialStateMethod();
        //                                               mutation.Apply(start);
        //                                               return start;
        //                                           });
        //    }
        //}



        //public class MultiTest<T> : IInitialStateAggregate<T>
        //{
        //    private readonly InitialStateCollection<T> _initialStates;
        //    private IInitialConditionFactory<T> _factory;

        //    public MultiTest(IInitialConditionFactory<T> factory)
        //    {
        //        _initialStates = new InitialStateCollection<T>();
        //        _factory = factory;
        //    }



        //    public IInitialStateAggregate<T> WithChange(IMutation<T> mutation)
        //    {
        //        var state = _factory.CreateCondition(mutation);
        //        _initialStates.Add(state);
        //        return this;
        //    }

        //    public ITestAct<TOutput> When<TOutput>(Func<T, TOutput> func)
        //    {
        //        return new TestAct<T, TOutput>(this, func);
        //    }
        //}



        //internal class MutationCollection<T> : IEnumerable<IMutation<T>>
        //{
        //    private readonly List<IMutation<T>> _mutations;

        //    public MutationCollection()
        //    {
        //        this._mutations = new List<IMutation<T>>();
        //    }

        //    public IEnumerator<IMutation<T>> GetEnumerator()
        //    {
        //        return _mutations.GetEnumerator();
        //    }

        //    IEnumerator IEnumerable.GetEnumerator()
        //    {
        //        return GetEnumerator();
        //    }

        //    public void Add(IMutation<T> mutation)
        //    {
        //        _mutations.Add(mutation);
        //    }
        //}


        //public class TestAct<T, TOutput> : ITestAct<TOutput>
        //{
        //    private readonly IInitialStateAggregate<T> _arrangement;
        //    private readonly Func<T, TOutput> _action;

        //    public TestAct(IInitialStateAggregate<T> arrangement, Func<T, TOutput> action)
        //    {
        //        if (arrangement == null) throw new ArgumentNullException("arrangement");
        //        if (action == null) throw new ArgumentNullException("action");
        //        _arrangement = arrangement;
        //        _action = action;
        //    }

        //    public ITestable Then(Action<TOutput, string> assertion)
        //    {
        //        return new Testable<T, TOutput>(_arrangement, _action, assertion);
        //    }
        //}

        //public class Testable<T, TOutput> : ITestable
        //{
        //    public Testable(IInitialStateAggregate<T> arrangement, Func<T, TOutput> action, Action<TOutput, string> assertion)
        //    {
        //    }

        //    public void Launch()
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public interface IInitialState<T>
        //{
        //    IInitialStateAggregate<T> WithChange(IMutation<T> mutation);
        //}

        //public interface IInitialStateAggregate<T> : IInitialState<T>
        //{
        //    ITestAct<TOutput> When<TOutput>(Func<T, TOutput> func);
        //}

        //public interface ITestAct<TOutput>
        //{
        //    ITestable Then(Action<TOutput, string> assertion);
        //}

        //public interface ITestable
        //{
        //    void Launch();
        //}

        //public static class TestInitialArrangeExtensions
        //{
        //    public static IInitialStateAggregate<T> WithChange<T>(this IInitialState<T> arranged, string condition, Action<T> modification)
        //    {
        //        var mutation = new Mutation<T>(condition, modification);
        //        return arranged.WithChange(mutation);
        //    }
        //}
    }
}
