using System;
using System.Collections.Generic;

namespace PolyTest.Tests.InitialStates.Fluent
{
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



    public interface IInitialStateCollectionFromStartingPoint<T> : IStateCollection<T>
    {
        ITestCase<T> StartingPoint { get; }
        void Add(IMutation<T> mutation);

    }


    public class InitialStateCollectionFromStartingPoint<T> : IInitialStateCollectionFromStartingPoint<T>
    {
        private readonly ITestCase<T> _startingPoint;
        private readonly InitialStateCollection<T> _internalCollection;

        public InitialStateCollectionFromStartingPoint(ITestCase<T> startingPoint)
        {
            _startingPoint = startingPoint;
            _internalCollection = new InitialStateCollection<T>();
        }

        public ITestCase<T> StartingPoint { get { return _startingPoint; } }

        public void Add(IMutation<T> mutation)
        {
            _internalCollection.Add(new SequentialMutations<T>(_startingPoint, mutation));
        }

        public IIndividualTestExecutionInformationCollection<TResult> Execute<TResult>(Func<T, TResult> testMethod)
        {
            return _internalCollection.Execute(testMethod);
        }
    }
}