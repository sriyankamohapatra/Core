using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Context.Fakes;
using Sfa.Core.Logging;
using Sfa.Core.Logging.Fakes;
using Sfa.Core.Testing;

namespace Sfa.Core.Context
{
    [TestClass]
    public class ApplicationContextTests // Can't inherit from base tests as this uses ApplicationContext!
    {
        #region Setup

        [TestMethod, TestCategory("Unit")]
        public void Setup()
        {
            // Act
            var loggerSet = false;
            var networkContextSet = false;

            var stubContextStorage = new StubIContextStorage();
            var stubLogger = new StubILogger();
            var stubNetworkContext = new StubINetworkContext();

            stubContextStorage.SetDataOf1StringM0<ILogger>((key, value) =>
            {
                loggerSet = true;
            });
            stubContextStorage.SetDataOf1StringM0<INetworkContext>((key, value) =>
            {
                networkContextSet = true;
            });

            // Act
            ApplicationContext.Setup(stubContextStorage, stubLogger, stubNetworkContext);

            // Assert
            loggerSet.ShouldHaveSameValueAs(true);
            networkContextSet.ShouldHaveSameValueAs(true);
        }

        #endregion


        #region Property Access


        [TestMethod, TestCategory("Unit")]
        public void Logger()
        {
            // Act
            var stubContextStorage = new StubIContextStorage();
            var stubLogger = new StubILogger();
            var stubNetworkContext = new StubINetworkContext();

            stubContextStorage.GetDataOf1String<ILogger>(key => stubLogger);
            ApplicationContext.Setup(stubContextStorage, stubLogger, stubNetworkContext);

            // Act
            var actual = ApplicationContext.Logger;

            // Assert
            actual.ShouldHaveSameValueAs(stubLogger);
        }


        [TestMethod, TestCategory("Unit")]
        public void NetworkContext()
        {
            // Act
            var stubContextStorage = new StubIContextStorage();
            var stubLogger = new StubILogger();
            var stubNetworkContext = new StubINetworkContext();

            stubContextStorage.GetDataOf1String<INetworkContext>(key => stubNetworkContext);
            ApplicationContext.Setup(stubContextStorage, stubLogger, stubNetworkContext);

            // Act
            var actual = ApplicationContext.NetworkContext;

            // Assert
            Assert.AreEqual(stubNetworkContext, actual);
        }

        #endregion


        #region Set


        [TestMethod, TestCategory("Unit")]
        public void Set()
        {
            // Act
            var valueSet = false;

            var stubContextStorage = new StubIContextStorage();

            stubContextStorage.SetDataOf1StringM0<string>((key, value) =>
            {
                valueSet = value == "some value";
            });
            ApplicationContext.Setup(stubContextStorage, null, null);

            // Act
            ApplicationContext.Set("some value");

            // Assert
            valueSet.ShouldHaveSameValueAs(true);
        }

        #endregion


        #region Set


        [TestMethod, TestCategory("Unit")]
        public void Get()
        {
            // Act
            var stubContextStorage = new StubIContextStorage();

            stubContextStorage.GetDataOf1String(key => "some value");
            ApplicationContext.Setup(stubContextStorage, null, null);

            // Act
            var actual = ApplicationContext.Get<string>();

            // Assert
            actual.ShouldHaveSameValueAs("some value");
        }

        #endregion


        #region TearDown


        [TestMethod, TestCategory("Unit")]
        public void TearDown()
        {
            // Act
            var keysToDispose = new List<string>();

            var stubContextStorage = new StubIContextStorage();
            var stubLogger = new StubILogger();
            var stubNetworkContext = new StubINetworkContext();

            stubContextStorage.CleanAndDisposeStringArray = keys => { keysToDispose.AddRange(keys); };
            ApplicationContext.Setup(stubContextStorage, stubLogger, stubNetworkContext);
            ApplicationContext.Set("some value");

            // Act
            ApplicationContext.TearDown();

            // Assert
            keysToDispose.ShouldHaveSameValueAs(new List<string> { "logger", "network", typeof(string).FullName});
        }

        #endregion
    }
}