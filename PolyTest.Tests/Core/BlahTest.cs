using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PolyTest.Tests.Core
{
    [TestClass]
    public class BlahTest
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


            results.ForEach((str, val) => AssertIsNotFive(val, str));


            // Act

            // Assert

        }

        [TestMethod]
        public void FluentFlatTest()
        {
            // Arrange
            var sut = new FakeSut();
            "Starting with IntProperty = 5".AsStartingPoint(() => new DummyItem(5))
                .Arrange("setting it to 4", d => { d.IntProperty = 4; })
                .Arrange("setting it to 3", d => { d.IntProperty = 3; })
            .Act(it => sut.DoIt(it))
            .Assert((str, val) => AssertIsNotFive(val, str));

            // Act

            // Assert

        }

        [TestMethod]
        public void FluentNestedTest()
        {
            // Arrange
            var sut = new FakeSut();
            "Starting with IntProperty = 5".AsStartingPoint(() => new DummyItem(5))
                .Arrange("setting it to 4", d => { d.IntProperty = 4; }, 
                    andThen => andThen.IgnoringRoot()
                        .With(new Mutation<DummyItem>("setting bool to false", d => { d.BoolProperty = false; }))
                        .With(new Mutation<DummyItem>("setting bool to true", d => { d.BoolProperty = true; }))
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



        #region Sut to test our tests (sic)

        private class DummyItem
        {
            public DummyItem(int initial)
            {
                IntProperty = initial;
                BoolProperty = false;
            }

            public int IntProperty { get; set; }

            public bool BoolProperty { get; set; }

        }

        private class FakeSut
        {
            public int DoIt(DummyItem item)
            {
                return item.IntProperty;
            }

            public bool HasIt(DummyItem item)
            {
                return item.BoolProperty;
            }
        }

        /// <summary>
        /// Stupid custom Assert method
        /// </summary>
        private static void AssertIsNotFive(int actual, string extraInfo)
        {
            Assert.AreNotEqual(5, actual, extraInfo);
        }

        #endregion
    }

    public static class PolyTestExtensions
    {
        public static IInitialStateCollectionFromStartingPoint<T> AsStartingPoint<T>(this string description, Func<T> initializationMethod)
        {
            // TODO : invent description with reflection ?
            var start =  new StartingPoint<T>(description, initializationMethod);
            return new InitialStateCollectionFromStartingPoint<T>(start);
        }


        public static IInitialStateCollectionFromStartingPoint<T> Arrange<T>(this IInitialStateCollectionFromStartingPoint<T> collection,
                                                           string mutationDescription, Action<T> modification)
        {
            collection.Add(new Mutation<T>(mutationDescription, modification));
            return collection;
        }

        public static IInitialStateCollectionFromStartingPoint<T> Arrange<T>(this IInitialStateCollectionFromStartingPoint<T> collection,
                                                   string mutationDescription, Action<T> modification,
                                                    Func<MutationList<T>, MutationList<T>> nested )
        {
            var commonMutationToApplyToAllNested = new Mutation<T>(mutationDescription, modification);

            var populatedNested = nested(new MutationList<T>());
            foreach (var mutation in populatedNested.Compose(commonMutationToApplyToAllNested))
            {
                // add all the nested mutations to the global list
                collection.Add(mutation);
            }
            return collection;
        } 

        public static IIndividualTestExecutionInformationCollection<TResult> Act<T, TResult>(
            this IInitialStateCollectionFromStartingPoint<T> startingPoints,
            Func<T, TResult> testedMethod)
        {
            return startingPoints.Execute(testedMethod);
        }

        public static void Assert<TResult>(
    this IIndividualTestExecutionInformationCollection<TResult> result,
    Action<string, TResult> assertion)
        {
            result.ForEach(assertion);
        }
    }

    public class MutationList<T>
    {
        private readonly List<IMutation<T>> _mutations;
        private bool _shouldConsiderParentAsACondition;

        public MutationList()
        {
            _mutations = new List<IMutation<T>>();
            _shouldConsiderParentAsACondition = true; // considerMe a condition
        }

        public MutationList<T> With(IMutation<T> mutationToAdd)
        {
            _mutations.Add(mutationToAdd);
            return this;
        } 

        public IEnumerable<IMutation<T>> Compose(IMutation<T> root)
        {
            if (_shouldConsiderParentAsACondition)
                yield return new Mutation<T>(root.Description + " (first before nested conditions)", (a) => root.Apply(a));
            foreach (var mutation in _mutations)
            {
                // add all the nested mutations to the global list
                var composedMutation = new CompositeMutation<T>(root, mutation);
                yield return composedMutation;
            }
        }

        public MutationList<T> IgnoringRoot(bool shouldIgnore = true)
        {
            this._shouldConsiderParentAsACondition = !shouldIgnore;
            return this;
        }
    }

    public class CompositeMutation<T> : IMutation<T>
    {
        private readonly IMutation<T> _root;
        private readonly IMutation<T> _second;

        public CompositeMutation(IMutation<T> root, IMutation<T> second)
        {
            _root = root;
            _second = second;
        }

        public string Description { get { return _root.Description + " AND " + _second.Description; } }
        public void Apply(T source)
        {
            _root.Apply(source);
            _second.Apply(source);
        }
    }

    public interface IStateCollection<T>
    {
        IIndividualTestExecutionInformationCollection<TResult> Execute<TResult>(Func<T, TResult> testMethod);
    }

    public interface IInitialStateCollectionFromStartingPoint<T> : IStateCollection<T>
    {
        IInitialization<T> StartingPoint { get; } 
        void Add(IMutation<T> mutation);
        
    }

    public class InitialStateCollectionFromStartingPoint<T> : IInitialStateCollectionFromStartingPoint<T>
    {
        private readonly IInitialization<T> _startingPoint;
        private readonly InitialStateCollection<T> _internalCollection;

        public InitialStateCollectionFromStartingPoint(IInitialization<T> startingPoint)
        {
            _startingPoint = startingPoint;
            _internalCollection = new InitialStateCollection<T>();
        }

        public IInitialization<T> StartingPoint { get { return _startingPoint; } }

        public void Add(IMutation<T> mutation)
        {
            _internalCollection.Add(new SequentialMutations<T>(_startingPoint, mutation));
        }

        public IIndividualTestExecutionInformationCollection<TResult> Execute<TResult>(Func<T, TResult> testMethod)
        {
            return _internalCollection.Execute(testMethod);
        }
    }


    /// <summary>
    /// Represents a way of preparing an instance of <typeparamref name="T"/> prior to a test
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IInitialization<T>
    {
        /// <summary>
        /// Initialize an instance of <typeparamref name="T"/> for testing
        /// </summary>
        /// <returns></returns>
        T Prepare();

        /// <summary>
        /// A human-driendly description of the state we reach with this Initialization
        /// </summary>
        string Description { get; }
    }

    /// <summary>
    /// An initial state that is reached by applying a series of mutations from an initial state
    /// </summary>
    public class SequentialMutations<T> : IInitialization<T>
    {
        private readonly IInitialization<T> _initialization;
        private readonly List<IMutation<T>> _mutations;

        public SequentialMutations(IInitialization<T> initialization, params IMutation<T>[] mutations)
        {
            _initialization = initialization;
            _mutations = mutations.ToList();
        }

        public T Prepare()
        {
            var start = _initialization.Prepare();
            foreach (var mutation in _mutations)
            {
                mutation.Apply(start);
            }
            return start;
        }

        public string Description
        {
            get
            {
                var sb = new StringBuilder(_initialization.Description);
                foreach (var mutation in _mutations)
                {
                    sb.Append(" AND " + mutation.Description);
                }
                return sb.ToString();
            }
        }

    }

    /// <summary>
    /// The starting point of a test. Represents the root element of type <typeparamref name="T"/> on which we apply variations for testing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StartingPoint<T> : IInitialization<T>
    {
        private readonly string _description;
        private readonly Func<T> _factoryMethod;

        public StartingPoint(string description, Func<T> factoryMethod)
        {
            _description = description;
            _factoryMethod = factoryMethod;
        }

        public string Description
        {
            get { return _description; }
        }

        public T Prepare()
        {
            return _factoryMethod();
        }
    }


    public interface IInitialStateCollection<T> : IStateCollection<T>
    {
        void Add(IInitialization<T> state);
    }

    internal class InitialStateCollection<T> : IInitialStateCollection<T>
    {
        private List<IInitialization<T>> _setups;

        public InitialStateCollection()
        {
            _setups = new List<IInitialization<T>>();
        }


        public void Add(IInitialization<T> state)
        {
            _setups.Add(state);
        }

        public IIndividualTestExecutionInformationCollection<TResult> Execute<TResult>(Func<T, TResult> testMethod)
        {
            return
                new IndividualTestExecutionInformationCollection<TResult>(
                    _setups.Select(
                        (initial, idx) =>
                        new IndividualTestExecutionInformation<TResult>(idx, initial.Description,
                                                                        testMethod(initial.Prepare()))));
        }
    }

    internal class IndividualTestExecutionInformationCollection<TResult> : IIndividualTestExecutionInformationCollection<TResult>
    {
        private readonly IEnumerable<IndividualTestExecutionInformation<TResult>> _results;

        public IndividualTestExecutionInformationCollection(IEnumerable<IndividualTestExecutionInformation<TResult>> results)
        {
            _results = results;
        }

        public void ForEach(Action<string, TResult> assertion)
        {
            foreach (var individualTestExecutionInformation in _results)
            {
                assertion(individualTestExecutionInformation.TestCaseDescription,
                          individualTestExecutionInformation.Result);
            }
        }
    }

    public interface IIndividualTestExecutionInformationCollection<TResult>
    {
        void ForEach(Action<string, TResult> assertion);
    }

    public interface IIndividualTestExecutionInformation<TResult>
    {

        int Index { get; }
        string TestCaseDescription { get; }
        TResult Result { get; }

    }

    class IndividualTestExecutionInformation<TResult> : IIndividualTestExecutionInformation<TResult>
    {
        public IndividualTestExecutionInformation(int index, string description, TResult resultValue)
        {
            Index = index;
            TestCaseDescription = description;
            Result = resultValue;
        }

        public int Index { get; private set; }
        public string TestCaseDescription { get; private set; }
        public TResult Result { get; private set; }
    }


    public interface IMutation<T>
    {
        string Description { get; }
        void Apply(T source);
    }

    public class Mutation<T> : IMutation<T>
    {
        private readonly string _mutationDescription;
        private readonly Action<T> _modificationToMake;

        public Mutation(string mutationDescription, Action<T> modificationToMake)
        {
            _mutationDescription = mutationDescription;
            _modificationToMake = modificationToMake;
        }

        public string Description
        {
            get { return _mutationDescription; }
        }

        public void Apply(T source)
        {
            _modificationToMake(source);
        }
    }

}
