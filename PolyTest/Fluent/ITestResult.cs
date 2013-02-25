namespace PolyTest.Fluent
{
    public interface ITestResult<T, out TResult> : ITestResult<T>
    {
        new TResult Result { get; }
    }

    public interface ITestResult<T>
    {
        object Result { get; }
        ITestTreeNode<T> TreeNode { get; }
        bool IsSuccess { get; }
        bool HasResult { get; }
        string ResultMessage { get; }
    }
}