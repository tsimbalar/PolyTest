namespace PolyTest
{
    /// <summary>
    ///  TestComposite, that is, a node with a collection of nodes, where each child node provide a mutation to apply to the parent
    /// </summary>
    public interface ITestComposite<T> : ITestComponent<T>
    {
        /// <summary>
        /// Add a child to this node
        /// </summary>
        /// <param name="child"></param>
        void Add(ITestComponent<T> child);

        /// <summary>
        /// Indicates whether the Enumerate() method returns an element for this node, or if it should just return its children
        /// </summary>
        bool IncludeSelfInEnumeration { get; set; }
    }
}