using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sfa.Core.Testing
{
    [TestClass]
    public class MoqExtensionsTests : BaseMoqApplicationLogicTest
    {
        [TestMethod, TestCategory("Unit")]
        public void MoqExtensions_ContstantExpression()
        {
            // Arrange
            var mock = NewMock<ISpike>();

            mock.Expected(o => o.TakeSomeParams("tim"));

            // Act
            mock.Object.TakeSomeParams("tim");
        }


        [TestMethod, TestCategory("Unit")]
        public void Moq_MemberAccessExpression()
        {
            // Arrange
            var mock = NewMock<ISpike>();
            var poco = new Poco { Name = "tim" };

            mock.Setup(o => o.TakeSomeParams(poco.Name));

            // Act
            mock.Object.TakeSomeParams("tim");
        }


        [TestMethod, TestCategory("Unit")]
        public void MoqExtensions_MemberAccessExpression()
        {
            // Arrange
            var mock = NewMock<ISpike>();
            var poco = new Poco { Name = "tim" };

            mock.Expected(o => o.TakeSomeParams(poco.Name));

            // Act
            mock.Object.TakeSomeParams("tim");
        }


        public class Poco : BasePoco
        {
            public string Name { get; set; }
        }

        public class BasePoco : ISomething
        {

        }

        public interface ISomething
        {
        }

        public interface ISpike
        {
            void TakeSomeParams(string giles);
        }
    }
}
