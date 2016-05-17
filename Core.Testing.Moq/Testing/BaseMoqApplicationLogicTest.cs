using Moq;
using Sfa.Core.Equality;

namespace Sfa.Core.Testing
{
    /// <summary>
    /// Base class for logical tests that use the Moq mocking framework.
    /// </summary>
    public class BaseMoqApplicationLogicTest : BaseTest
    {
        private MockRepository _mockRepository;

        /// <summary>
        /// Sets up the moq repo afreash for each test.
        /// </summary>
        protected override void SetUpEachTest()
        {
            base.SetUpEachTest();
            MoqExtensions.SetEqualityComparer(new FieldValueEqualityComparer());
            FieldValueEqualityComparer.AddFieldValueEqualityComparer(new MoqFieldValueEqualityComparer());
            _mockRepository = new MockRepository(MockBehavior.Strict);
        }

        /// <summary>
        /// Ensures that the Moqs were called correctly at the end of a test.
        /// </summary>
        protected override void TearDownEachTest()
        {
            base.TearDownEachTest();

            _mockRepository.VerifyAll();
        }

        /// <summary>
        /// Creates a new mock of the specified type.
        /// </summary>
        /// <typeparam name="T">The type to mock.</typeparam>
        /// <returns>The new mock.</returns>
        protected Mock<T> NewMock<T>()
            where T : class
        {
            return _mockRepository.Create<T>();
        }
    }
}