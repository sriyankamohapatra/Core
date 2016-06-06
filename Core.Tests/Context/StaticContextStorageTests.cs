using System;
using System.Collections.Generic;
using System.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Context
{
    [TestClass]
    public class StaticContextStorageTests // Can't inherit from BaseTest as this will overwrite ApplicationContext and change the static storage
    {
        #region GetData

        [TestMethod, TestCategory("Unit")]
        public void GetData()
        {
            // Arrange
            var expected = new DateTime(2000, 1, 2);
            var storage = new Dictionary<string, object>();
            var componentUnderTest = new StaticContextStorage(storage);

            storage["now"] = expected;

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
            var storage = new Dictionary<string, object>();
            var componentUnderTest = new StaticContextStorage(storage);

            // Act
            componentUnderTest.SetData("now", expected);

            // Assert
            storage["now"].ShouldHaveSameValueAs(expected);
        }

        #endregion


        #region CleanAndDispose

        [TestMethod, TestCategory("Unit")]
        public void CleanAndDispose()
        {
            // Arrange
            var called = false;
            var storage = new Dictionary<string, object>();
            var shouldBeDisposed = new StubIDisposable
            {
                Dispose = () => called = true
            };
            var componentUnderTest = new StaticContextStorage(storage);
            storage["shouldBeDisposed"] = shouldBeDisposed;

            // Act
            componentUnderTest.CleanAndDispose("shouldBeDisposed");

            // Assert
            called.ShouldHaveSameValueAs(true);
        }

        #endregion
    }
}