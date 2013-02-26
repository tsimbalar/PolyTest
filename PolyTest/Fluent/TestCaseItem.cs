using System;

namespace PolyTest.Fluent
{
    internal class TestCaseItem<T> : ITestCaseInformation, ITestCase<T>
    {
        private readonly int _index;
        private readonly ITestCase<T> _testCase;

        public TestCaseItem(int index, ITestCase<T> testCase)
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


        internal TestResult<TResult> Test<TResult>(Func<T, TResult> act, Action<TResult> assert)
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
                        return TestResultFactory.AssertFailed(this, result, e);
                    }
                    return TestResultFactory.Success(this, result);
                }
                catch (Exception e)
                {
                    return TestResultFactory.ActFailed<TResult>(this, e);
                }
            }
            catch (Exception e)
            {
                return TestResultFactory.ArrangeFailed<TResult>(this, e);
            }

        }

        public override string ToString()
        {
            return string.Format("#{0} - {1}", Index, Description);
        }
    }
}