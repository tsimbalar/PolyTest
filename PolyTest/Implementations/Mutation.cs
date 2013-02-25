using System;

namespace PolyTest.Implementations
{
    /// <summary>
    /// Implementation of a Mutation simply based on a description and a method passed in the constructor
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class Mutation<T> : IMutation<T>
    {
        private readonly string _mutationDescription;
        private readonly Action<T> _mutationToApply;

        public Mutation(string mutationDescription, Action<T> mutationToApply)
        {
            if (mutationDescription == null) throw new ArgumentNullException("mutationDescription");
            if (mutationToApply == null) throw new ArgumentNullException("mutationToApply");
            _mutationDescription = mutationDescription;
            _mutationToApply = mutationToApply;
        }

        public string Description
        {
            get { return _mutationDescription; }
        }

        public void Apply(T source)
        {
            _mutationToApply(source);
        }
    }
}