namespace PolyTest
{
    /// <summary>
    /// A mutation is a documented transformation of a given input.
    /// </summary>
    /// <typeparam name="T">the type on which the mutation applies</typeparam>
    public interface IMutation<in T>
    {
        /// <summary>
        /// A human-friendly description of the mutation
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Apply the given mutation to the source passed as parameter
        /// </summary>
        /// <param name="source">the input to mutate</param>
        void Apply(T source);
    }
}