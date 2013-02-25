using System;

namespace PolyTest.Fluent
{
    internal class TestTreeNode<T> : ITestTreeNode<T>
    {
        private readonly int _index;
        private readonly ITestCase<T> _testCase;

        public TestTreeNode(int index, ITestCase<T> testCase)
        {
            if (testCase == null) throw new ArgumentNullException("testCase");
            _index = index;
            _testCase = testCase;
        }

        public string Description { get { return _testCase.Description; } }

        public int Index
        {
            get { return _index; }
        }

        public T Arrange()
        {
            return _testCase.Arrange();
        }


        internal TestResult<T, TResult> TestInternal<TResult>(Func<T, TResult> act, Action<TResult> assert)
        {
            try
            {
                var init = Arrange();
                try
                {
                    var result = act(init);

                    try
                    {
                        assert(result);
                    }
                    catch (Exception e)
                    {
                        return TestResult.AssertFailed(this, result, e);
                    }
                    return TestResult.Success(this, result);
                }
                catch (Exception e)
                {
                    return TestResult.ActFailed<T, TResult>(this, e);
                }
            }
            catch (Exception e)
            {
                return TestResult.ArrangeFailed<T, TResult>(this, e);
            }

        }

        public ITestResult<T, TResult> Test<TResult>(Func<T, TResult> act, Action<TResult> assert)
        {
            return TestInternal(act, assert);
        }

        public override string ToString()
        {
            return string.Format("#{0} - {1}", Index, Description);
        }
    }
}