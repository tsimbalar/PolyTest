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
            testCases.Add(new SequentialMutations<DummyItem>(initialCondition,
                new Mutation<DummyItem>("adding 2", d => { d.IntProperty = d.IntProperty + 2; }),
                new Mutation<DummyItem>("removing 1", d => { d.IntProperty = d.IntProperty - 1; })));


            var results = testCases.Execute(it => sut.DoIt(it));


            results.ForEach((str, val) => AssertIsNotFive(val, str));
            

            // Act

            // Assert

        }



        #region Sut to test our tests (sic)

        private class DummyItem
        {
            public DummyItem(int initial)
            {
                IntProperty = initial;
            }

            public int IntProperty { get; set; }


        }

        private class FakeSut
        {
            public int DoIt(DummyItem item)
            {
                return item.IntProperty;
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



    

    internal interface IInitialStateCollection<T>
    {
        void Add(IInitialization<T> state);
        IIndividualTestExecutionInformationCollection<TResult> Execute<TResult>(Func<T, TResult> testMethod);
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

        int Index { get;}
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
