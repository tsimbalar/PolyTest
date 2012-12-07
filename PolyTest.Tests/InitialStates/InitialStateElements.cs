using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolyTest.Tests.InitialStates
{


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


    public interface IMutation<T>
    {
        string Description { get; }
        void Apply(T source);
    }

    public interface IIndividualTestExecutionInformationCollection<TResult>
    {
        void ForEach(Action<string, TResult> assertion);
    }

    public interface IStateCollection<T>
    {
        IIndividualTestExecutionInformationCollection<TResult> Execute<TResult>(Func<T, TResult> testMethod);
    }

    public interface IInitialStateCollection<T> : IStateCollection<T>
    {
        void Add(IInitialization<T> state);
    }

    public interface IIndividualTestExecutionInformation<TResult>
    {

        int Index { get; }
        string TestCaseDescription { get; }
        TResult Result { get; }

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



    public class IndividualTestExecutionInformation<TResult> : IIndividualTestExecutionInformation<TResult>
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
