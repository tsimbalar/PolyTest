namespace PolyTest.Fluent
{
    public interface ITestRootFluent<T> : ITestCompositeFluent<T>
    {
        /// <summary>
        /// Root is not considered by default, but it can be ...
        /// </summary>
        /// <returns></returns>
        ITestRootFluent<T> IncludeSelf(string reason = null);
    }
}