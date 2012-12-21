namespace PolyTest.Tree
{
    /// <summary>
    /// The composite in the tree, that is, a component with children
    /// </summary>
    public interface ITestComposite<T> : ITestComponent<T>
    {
        void Add(ITestComponent<T> child);

        /// <summary>
        /// Indicates whether the Enumerate() method returns an element for this node, or if it should just return its children
        /// </summary>
        bool IncludeSelfInEnumeration { get; set; }
    }
}