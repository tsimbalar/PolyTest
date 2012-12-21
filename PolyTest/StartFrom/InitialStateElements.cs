using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolyTest.StartFrom
{
    public interface IIndividualTestExecutionInformationCollection<out TResult>
    {
        void ForEach(Action<string, TResult> assertion);
    }

    public interface IStateCollection<T>
    {
        IIndividualTestExecutionInformationCollection<TResult> Execute<TResult>(Func<T, TResult> testMethod);
    }

    public interface IInitialStateCollection<T> : IStateCollection<T>
    {
        void Add(ITestCase<T> state);
    }

    public interface IIndividualTestExecutionInformation<out TResult>
    {

        int Index { get; }
        string TestCaseDescription { get; }
        TResult Result { get; }

    }


    /// <summary>
    /// The starting point of a test. Represents the root element of type <typeparamref name="T"/> on which we apply variations for testing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class StartingPoint<T> : ITestCase<T>
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

        public T Arrange()
        {
            return _factoryMethod();
        }
    }


    /// <summary>
    /// An initial state that is reached by applying a series of mutations from an initial state
    /// </summary>
    internal class SequentialMutations<T> : ITestCase<T>
    {
        private readonly ITestCase<T> _initialization;
        private readonly List<IMutation<T>> _mutations;

        public SequentialMutations(ITestCase<T> initialization, params IMutation<T>[] mutations)
        {
            _initialization = initialization;
            _mutations = mutations.ToList();
        }

        public T Arrange()
        {
            var start = _initialization.Arrange();
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


    internal class InitialStateCollection<T> : IInitialStateCollection<T>
    {
        private readonly List<ITestCase<T>> _setups;

        public InitialStateCollection()
        {
            _setups = new List<ITestCase<T>>();
        }


        public void Add(ITestCase<T> state)
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
                                                                        testMethod(initial.Arrange()))));
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



    internal class IndividualTestExecutionInformation<TResult> : IIndividualTestExecutionInformation<TResult>
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
}
