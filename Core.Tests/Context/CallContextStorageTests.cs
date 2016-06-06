using System;
using System.Fakes;
using System.Runtime.Remoting.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Context
{
    [TestClass]
    public class CallContextStorageTests : BaseTest
    {
        #region GetData

        [TestMethod, TestCategory("Unit")]
        public void GetData()
        {
            // Arrange
            var expected = new DateTime(2000, 1, 2);
            var componentUnderTest = new CallContextStorage();

            CallContext.LogicalSetData("now", expected);

            // Act
            var actual = componentUnderTest.GetData<DateTime>("now");

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        #endregion


        #region SetData

        [TestMethod, TestCategory("Unit")]
        public void SetData()
        {
            // Arrange
            var expected = new DateTime(2000, 1, 2);
            var componentUnderTest = new CallContextStorage();
            
            // Act
            componentUnderTest.SetData("now", expected);

            // Assert
            CallContext.LogicalGetData("now").ShouldHaveSameValueAs(expected);
        }

        #endregion


        #region CleanAndDispose

        [TestMethod, TestCategory("Unit")]
        public void CleanAndDispose()
        {
            // Arrange
            var called = false;
            var shouldBeDisposed = new StubIDisposable
            {
                Dispose = () => called = true
            };
            var componentUnderTest = new CallContextStorage();

            CallContext.LogicalSetData("shouldBeDisposed", shouldBeDisposed);

            // Act
            componentUnderTest.CleanAndDispose("shouldBeDisposed");

            // Assert
            called.ShouldHaveSameValueAs(true);
        }

        #endregion
    }
}