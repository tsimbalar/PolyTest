
namespace PolyTest.Tests.TestUtils
{
    #region Sut to test our tests (sic)

    public class FakeSut
        {
            public int DoIt(DummyItem item)
            {
                return item.IntProperty;
            }

            public bool HasIt(DummyItem item)
            {
                return item.BoolProperty;
            }
        }

        
        #endregion
}
