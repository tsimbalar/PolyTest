using System;

namespace PolyTest.Implementations.Fluent
{
    // TODO: get rid of that !
    static class TestRunner
    {
        public static TestResult<TResult> Run<T, TResult>(int index, ITestCase<T> testCase, Func<T, TResult> act, Action<TResult> assert)
        {
            if (testCase == null) throw new ArgumentNullException("testCase");


            var testCaseInformation = new TestCaseInformation(index, testCase.Description);
            T init;
            try
            {
                init = testCase.Arrange();
            }
            catch (Exception e)
            {
                return TestResultFactory.ArrangeFailed<TResult>(testCaseInformation, e);
            }


            TResult result;
            try
            {
                result = act(init);

            }
            catch (Exception e)
            {
                return TestResultFactory.ActFailed<TResult>(testCaseInformation, e);
            }


            try
            {
                assert(result);
            }
            catch (Exception e)
            {
                return TestResultFactory.AssertFailed(testCaseInformation, result, e);
            }

            // we reached this part . We are safe !
            return TestResultFactory.Success(testCaseInformation, result);
        } 
    }
}