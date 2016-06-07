using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Exceptions;
using Sfa.Core.Testing;

namespace Sfa.Core.Data
{
    [TestClass]
    public class CommandTests : BaseTest
    {
        #region Test Classes

        public class DefaultCommand : Command<string, string>
        {
            public bool OnAuthoriseCalled { get; set; }
            public bool OnAfterExecuteCalled { get; set; }
            public bool OnAfterExecuteCalledWithException { get; set; }
            public bool OnBeforeExecuteCalled { get; set; }
            public bool OnBeforeInitialiseTargetCalled { get; set; }
            public bool OnInitialiseTargetCalled { get; set; }
            public bool OnExecuteCalled { get; set; }

            protected override bool OnAuthorise()
            {
                OnAuthoriseCalled = true;
                return base.OnAuthorise();
            }

            protected override void OnAfterExecute(Exception exception = null)
            {
                OnAfterExecuteCalled = true;
                OnAfterExecuteCalledWithException = exception != null;
                base.OnAfterExecute(exception);
            }

            protected override void OnBeforeExecute()
            {
                OnBeforeExecuteCalled = true;
                base.OnBeforeExecute();
            }

            protected override void OnBeforeInitialiseTarget()
            {
                OnBeforeInitialiseTargetCalled = true;
                base.OnBeforeInitialiseTarget();
            }

            protected override void OnInitialiseTarget()
            {
                OnInitialiseTargetCalled = true;
                base.OnInitialiseTarget();
            }

            protected override void OnExecute()
            {
                OnExecuteCalled = true;
            }

            protected override IEnumerable<string> PropertiesToIgnoreForToString
            {
                get
                {
                    foreach (var prop in base.PropertiesToIgnoreForToString)
                    {
                        yield return prop;
                    }
                    yield return nameof(OnBeforeExecuteCalled);
                }
            }
        }

        public class UnauthorisedCommand : DefaultCommand
        {
            protected override bool OnAuthorise()
            {
                OnAuthoriseCalled = true;
                return false;
            }
        }

        public class ExceptionInExecuteCommand : DefaultCommand
        {
            protected override void OnExecute()
            {
                OnExecuteCalled = true;
                throw new Exception("dummy");
            }
        }

        #endregion


        #region Authorise

        [TestMethod, TestCategory("Unit")]
        public void Authorise()
        {
            // Arrange
            var componentUnderTest = new DefaultCommand();

            // Act
            var actual = componentUnderTest.Authorise();

            // Assert
            actual.ShouldHaveSameValueAs(true);
        }

        #endregion


        #region Execute


        [TestMethod, TestCategory("Unit")]
        public void Execute()
        {
            // Arrange
            var componentUnderTest = new DefaultCommand();

            // Act
            componentUnderTest.Execute();

            // Assert
            componentUnderTest.OnBeforeInitialiseTargetCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnInitialiseTargetCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnAuthoriseCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnBeforeExecuteCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnExecuteCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnAfterExecuteCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnAfterExecuteCalledWithException.ShouldHaveSameValueAs(false);
        }


        [TestMethod, TestCategory("Unit")]
        public void Execute_UnauthorisedCommand()
        {
            // Arrange
            var componentUnderTest = new UnauthorisedCommand();

            // Act
            try
            {
                componentUnderTest.Execute();
            }
            catch (UnauthorizedException)
            {
            }

            // Assert
            componentUnderTest.OnBeforeInitialiseTargetCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnInitialiseTargetCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnAuthoriseCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnBeforeExecuteCalled.ShouldHaveSameValueAs(false);
            componentUnderTest.OnExecuteCalled.ShouldHaveSameValueAs(false);
            componentUnderTest.OnAfterExecuteCalled.ShouldHaveSameValueAs(false);
            componentUnderTest.OnAfterExecuteCalledWithException.ShouldHaveSameValueAs(false);
        }


        [TestMethod, TestCategory("Unit")]
        public void Execute_ExceptionInExecuteCommand()
        {
            // Arrange
            var componentUnderTest = new ExceptionInExecuteCommand();

            // Act
            try
            {
                componentUnderTest.Execute();
            }
            catch (Exception)
            {
            }

            // Assert
            componentUnderTest.OnBeforeInitialiseTargetCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnInitialiseTargetCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnAuthoriseCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnBeforeExecuteCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnExecuteCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnAfterExecuteCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnAfterExecuteCalledWithException.ShouldHaveSameValueAs(true);
        }

        #endregion


        #region Property Access


        [TestMethod, TestCategory("Unit")]
        public void Target()
        {
            // Arrange
            var componentUnderTest = new DefaultCommand();

            // Act
            componentUnderTest.Target = "target";

            // Assert
            componentUnderTest.Target.ShouldHaveSameValueAs("target");
        }


        [TestMethod, TestCategory("Unit")]
        public void Result()
        {
            // Arrange
            var componentUnderTest = new DefaultCommand();

            // Act
            componentUnderTest.Result = "result";

            // Assert
            componentUnderTest.Result.ShouldHaveSameValueAs("result");
        }

        #endregion


        #region ToString


        [TestMethod, TestCategory("Unit")]
        public void ToStringTest()
        {
            // Arrange
            var componentUnderTest = new DefaultCommand();
            var expected = "[OnAuthoriseCalled:False] [OnAfterExecuteCalled:False] [OnAfterExecuteCalledWithException:False] [OnBeforeInitialiseTargetCalled:False] [OnInitialiseTargetCalled:False] [OnExecuteCalled:False]";

            // Act
            var actual = componentUnderTest.ToString();

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        #endregion
    }
}