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

        [TestMethod]
        public void CompositeTest()
        {
            // Arrange
            var sut = new FakeSut();

            var rootStartingAt5 = new TestRoot<DummyItem>("Starting with IntProperty = 5", () => new DummyItem(5));

            rootStartingAt5.Add(new TestLeaf<DummyItem>(rootStartingAt5, "setting it to 4", d => { d.IntProperty = 4; }));
            rootStartingAt5.Add(new TestLeaf<DummyItem>(rootStartingAt5, "setting it to 3", d => { d.IntProperty = 3; }));

            var adding2 = new TestComposite<DummyItem>(rootStartingAt5, "adding 2", d => { d.IntProperty = d.IntProperty + 2; }, includeInEnumeration:true);
            adding2.Add(new TestLeaf<DummyItem>(adding2, "removing 1", d => { d.IntProperty = d.IntProperty - 1; }));
            adding2.Add(new TestLeaf<DummyItem>(adding2, "removing 3", d => { d.IntProperty = d.IntProperty - 3; }));
            rootStartingAt5.Add(adding2);

            var adding3 = new TestComposite<DummyItem>(rootStartingAt5, "adding 3", d => { d.IntProperty = d.IntProperty + 3; }, includeInEnumeration: true);
            adding2.Add(new TestLeaf<DummyItem>(adding3, "removing 2", d => { d.IntProperty = d.IntProperty - 2; }));
            adding2.Add(new TestLeaf<DummyItem>(adding3, "removing 4", d => { d.IntProperty = d.IntProperty - 4; }));

            rootStartingAt5.Add(adding3);

            //Assert.AreEqual("", String.Join("\n", rootStartingAt5.Enumerate().ToList().Select(t => t.Description)));

            rootStartingAt5.Walk((testCaseDescription, arrange) =>
                                                   {
                                                       var initial = arrange();
                                                       var actual = sut.DoIt(initial);
                                                       AssertIsNotFive(actual, testCaseDescription);
                                                   });


            // Act

            // Assert
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
    
    /// <summary>
    /// What is a test case ? 
    /// Just a description and an initial state
    /// </summary>
    /// <typeparam name="T">The type that is initialized by the testcase</typeparam>
    public interface ITestCase<T>
    {
        String Description { get; }
        T Arrange();
    }

    /// <summary>
    /// The component that is the common part of every element in the tree
    /// </summary>
    /// <typeparam name="T">the type that the test cases initialize</typeparam>
    public interface ITestComponent<T> : ITestCase<T>
    {
        IEnumerable<ITestCase<T>> Enumerate();
        String NodeDescription { get; }
    }

    /// <summary>
    /// The composite in the tree, that is, a component with children
    /// </summary>
    public interface ITestComposite<T> : ITestComponent<T>
    {
        void Add(ITestComponent<T> child);
    }

    /// <summary>
    /// Default implementation of ITestComponent
    /// Generates Description based on parent description + this node's description
    /// Provides access to Parent for subclasses
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TestComponentBase<T> : ITestComponent<T>
    {
        private readonly ITestComposite<T> _parent;
        private readonly string _nodeDescription;

        /// <summary>
        /// Create a TestComponent
        /// </summary>
        /// <param name="parent">the parent of this node (can be null)</param>
        /// <param name="nodeDescription">the description for this node</param>
        protected TestComponentBase(ITestComposite<T> parent, string nodeDescription)
        {
            if (nodeDescription == null) throw new ArgumentNullException("nodeDescription");
            _parent = parent;
            _nodeDescription = nodeDescription;
        }

        /// <summary>
        /// The Description for this element. Based on the parent's description (if there is a parent) and this node's description
        /// </summary>
        public virtual string Description
        {
            get
            {
                if (Parent == null)
                {
                    return this._nodeDescription;
                }
                else
                {
                    return Parent.Description + " AND " + this._nodeDescription;
                }
            }
        }

        /// <summary>
        /// The parent of this component (can be null)
        /// </summary>
        protected ITestComposite<T> Parent { get { return _parent; } }

        /// <summary>
        /// Enumerate over the test cases of this component 
        /// </summary>
        public virtual IEnumerable<ITestCase<T>> Enumerate()
        {
            yield return this;
        }

        public string NodeDescription { get { return _nodeDescription; } }

        /// <summary>
        /// TestCase setup for this component
        /// </summary>
        /// <returns></returns>
        public abstract T Arrange();
    }

    /// <summary>
    /// Default implementation of the ITestComposite&lt;T&gt;
    /// Takes care of adding children and enumerating over its children
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TestCompositeBase<T> : TestComponentBase<T>, ITestComposite<T>
    {
        private readonly List<ITestComponent<T>> _children;

        /// <summary>
        /// Creates a composite
        /// </summary>
        /// <param name="parent">the parent of this node</param>
        /// <param name="description">description for this node</param>
        /// <param name="includeInEnumeration">when enumerating/walking over the tree, should this node be included, or should it just enumerate over its children ?</param>
        protected TestCompositeBase(ITestComposite<T> parent, string description, bool includeInEnumeration)
            : base(parent, description)
        {
            IncludeSelfInEnumeration = includeInEnumeration;
            _children = new List<ITestComponent<T>>();
        }

        /// <summary>
        /// Indicates whether the Enumerate() method returns an element for this node, or if it should just return its children
        /// </summary>
        protected bool IncludeSelfInEnumeration { get; set; }

        public void Add(ITestComponent<T> child)
        {
            _children.Add(child);
        }

        /// <summary>
        /// Enumerate over all the test cases, including the children and itself if IncludeSelfInEnumeration is true
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<ITestCase<T>> Enumerate()
        {
            if (IncludeSelfInEnumeration)
            {
                yield return this;
            }
            foreach (var testComponent in _children)
            {
                foreach (var testCase in testComponent.Enumerate())
                {
                    yield return testCase;
                }
            }
        }
    }

    /// <summary>
    /// The root of the tree of testcases
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TestRoot<T> : TestCompositeBase<T>
    {
        private readonly Func<T> _setup;

        /// <summary>
        /// Creates a root for the tree of testcases
        /// </summary>
        /// <param name="description">the descritpion of the initial condition</param>
        /// <param name="setup">how to obtain the initial element that is used as a base for all tests</param>
        public TestRoot(string description, Func<T> setup)
            : base(null, description, false /* do not enumerate over self by default*/)
        {
            if (setup == null) throw new ArgumentNullException("setup");
            _setup = setup;
        }

        /// <summary>
        /// Get a T in its initial state
        /// </summary>
        /// <returns></returns>
        public override T Arrange()
        {
            return _setup();
        }
    }

    /// <summary>
    /// TestComposite, that is, a node with a collection of nodes, where each child node provide a mutation to apply to the parent
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TestComposite<T> : TestCompositeBase<T>
    {
        private readonly Action<T> _mutation;

        /// <summary>
        /// Creates a TestComposite 
        /// </summary>
        /// <param name="parent">the parent for this component</param>
        /// <param name="description">description for this node</param>
        /// <param name="mutation">which mutation this element introduces</param>
        /// <param name="includeInEnumeration">should this node be considered a test case, or is it just a way of grouping the children ?</param>
        public TestComposite(ITestComposite<T> parent, string description, Action<T> mutation, bool includeInEnumeration)
            : base(parent, description, includeInEnumeration)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (mutation == null) throw new ArgumentNullException("mutation");
            _mutation = mutation;
        }

        /// <summary>
        /// Initialize a T starting from the parent's arrangement and applying this's mutation
        /// </summary>
        /// <returns></returns>
        public override T Arrange()
        {
            var startFrom = Parent.Arrange();
            _mutation(startFrom);
            return startFrom;
        }
    }

    /// <summary>
    /// The bottom element of the tree, a test case with no children
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TestLeaf<T> : TestComponentBase<T>
    {
        private readonly Action<T> _mutation;

        public TestLeaf(ITestComposite<T> parent, string description, Action<T> mutation)
            : base(parent, description)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (mutation == null) throw new ArgumentNullException("mutation");
            _mutation = mutation;
        }

        public override T Arrange()
        {
            var startFrom = Parent.Arrange();
            _mutation(startFrom);
            return startFrom;
        }
    }

    /// <summary>
    /// Extensions to walk over the testcases of a tree of testcases, providing access to initialization method and description of the testcase
    /// </summary>
    public static class TestCompositeExtensions
    {
        public static void Walk<T>(this ITestComponent<T> component, Action<string, Func<T>> action)
        {
            foreach (var testCase in component.Enumerate())
            {
                ITestCase<T> currentCase = testCase; // copy of the loop variable because it is referenced in a closure
                action(testCase.Description, currentCase.Arrange);
            }
        }
    }


    public static class PolyTestExtensions
    {
        public static IInitialStateCollectionFromStartingPoint<T> AsStartingPoint<T>(this string description, Func<T> initializationMethod)
        {
            // TODO : invent description with reflection ?
            var start = new StartingPoint<T>(description, initializationMethod);
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
                                                    Func<MutationList<T>, MutationList<T>> nested)
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
