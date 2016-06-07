using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Exceptions;
using Sfa.Core.Testing;

namespace Sfa.Core.Data
{
    [TestClass]
    public class AsyncCommandTests : BaseTest
    {
        #region Test Classes

        public class DefaultAsyncCommand : AsyncCommand<string, string>
        {
            public bool OnAuthoriseAsyncCalled { get; set; }
            public bool OnAfterExecuteAsyncCalled { get; set; }
            public bool OnAfterExecuteAsyncCalledWithException { get; set; }
            public bool OnBeforeExecuteAsyncCalled { get; set; }
            public bool OnBeforeInitialiseTargetAsyncCalled { get; set; }
            public bool OnInitialiseTargetAsyncCalled { get; set; }
            public bool OnExecuteAsyncCalled { get; set; }

            protected override Task<bool> OnAuthoriseAsync()
            {
                OnAuthoriseAsyncCalled = true;
                return base.OnAuthoriseAsync();
            }

            protected override Task OnAfterExecuteAsync(Exception exception = null)
            {
                OnAfterExecuteAsyncCalled = true;
                OnAfterExecuteAsyncCalledWithException = exception != null;
                return base.OnAfterExecuteAsync(exception);
            }

            protected override Task OnBeforeExecuteAsync()
            {
                OnBeforeExecuteAsyncCalled = true;
                return base.OnBeforeExecuteAsync();
            }

            protected override Task OnBeforeInitialiseTargetAsync()
            {
                OnBeforeInitialiseTargetAsyncCalled = true;
                return base.OnBeforeInitialiseTargetAsync();
            }

            protected override Task OnInitialiseTargetAsync()
            {
                OnInitialiseTargetAsyncCalled = true;
                return base.OnInitialiseTargetAsync();
            }

            protected override Task OnExecuteAsync()
            {
                OnExecuteAsyncCalled = true;
                return Task.FromResult(0);
            }
        }

        public class UnauthorisedAsyncCommand : DefaultAsyncCommand
        {
            protected override Task<bool> OnAuthoriseAsync()
            {
                OnAuthoriseAsyncCalled = true;
                return Task.FromResult(false);
            }
        }

        public class ExceptionInExecuteAsyncCommand : DefaultAsyncCommand
        {
            protected override Task OnExecuteAsync()
            {
                OnExecuteAsyncCalled = true;
                throw new Exception("dummy");
            }
        }

        #endregion


        #region AuthoriseAsync

        [TestMethod, TestCategory("Unit")]
        public async Task AuthoriseAsync()
        {
            // Arrange
            var componentUnderTest = new DefaultAsyncCommand();

            // Act
            var actual = await componentUnderTest.AuthoriseAsync();

            // Assert
            actual.ShouldHaveSameValueAs(true);
        }

        #endregion


        #region ExecuteAsync


        [TestMethod, TestCategory("Unit")]
        public async Task ExecuteAsync()
        {
            // Arrange
            var componentUnderTest = new DefaultAsyncCommand();

            // Act
            await componentUnderTest.ExecuteAsync();

            // Assert
            componentUnderTest.OnBeforeInitialiseTargetAsyncCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnInitialiseTargetAsyncCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnAuthoriseAsyncCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnBeforeExecuteAsyncCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnExecuteAsyncCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnAfterExecuteAsyncCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnAfterExecuteAsyncCalledWithException.ShouldHaveSameValueAs(false);
        }


        [TestMethod, TestCategory("Unit")]
        public async Task ExecuteAsync_UnauthorisedAsyncCommand()
        {
            // Arrange
            var componentUnderTest = new UnauthorisedAsyncCommand();

            // Act
            try
            {
                await componentUnderTest.ExecuteAsync();
            }
            catch (UnauthorizedException)
            {
            }

            // Assert
            componentUnderTest.OnBeforeInitialiseTargetAsyncCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnInitialiseTargetAsyncCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnAuthoriseAsyncCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnBeforeExecuteAsyncCalled.ShouldHaveSameValueAs(false);
            componentUnderTest.OnExecuteAsyncCalled.ShouldHaveSameValueAs(false);
            componentUnderTest.OnAfterExecuteAsyncCalled.ShouldHaveSameValueAs(false);
            componentUnderTest.OnAfterExecuteAsyncCalledWithException.ShouldHaveSameValueAs(false);
        }


        [TestMethod, TestCategory("Unit")]
        public async Task ExecuteAsync_ExceptionInExecuteAsyncCommand()
        {
            // Arrange
            var componentUnderTest = new ExceptionInExecuteAsyncCommand();
            var exceptionThrown = false;

            // Act
            try
            {
                await componentUnderTest.ExecuteAsync();
            }
            catch (Exception)
            {
                exceptionThrown = true;
            }

            // Assert
            Assert.IsTrue(exceptionThrown);
            componentUnderTest.OnBeforeInitialiseTargetAsyncCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnInitialiseTargetAsyncCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnAuthoriseAsyncCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnBeforeExecuteAsyncCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnExecuteAsyncCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnAfterExecuteAsyncCalled.ShouldHaveSameValueAs(true);
            componentUnderTest.OnAfterExecuteAsyncCalledWithException.ShouldHaveSameValueAs(true);
        }

        #endregion


        #region Property Access


        [TestMethod, TestCategory("Unit")]
        public void Target()
        {
            // Arrange
            var componentUnderTest = new DefaultAsyncCommand();

            // Act
            componentUnderTest.Target = "target";

            // Assert
            componentUnderTest.Target.ShouldHaveSameValueAs("target");
        }


        [TestMethod, TestCategory("Unit")]
        public void Result()
        {
            // Arrange
            var componentUnderTest = new DefaultAsyncCommand();

            // Act
            componentUnderTest.Result = "result";

            // Assert
            componentUnderTest.Result.ShouldHaveSameValueAs("result");
        }

        #endregion
    }
}