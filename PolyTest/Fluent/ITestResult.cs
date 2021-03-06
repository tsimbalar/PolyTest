namespace PolyTest.Fluent
{
    public interface ITestResult<out TResult>
    {
        TResult Result { get; }
        ITestCaseInformation TestCase { get; }
        bool IsSuccess { get; }
        bool HasResult { get; }
        string ResultMessage { get; }
    }
}