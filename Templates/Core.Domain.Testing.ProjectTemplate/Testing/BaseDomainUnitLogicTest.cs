using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moq;
using Sfa.Core.Context;
using Sfa.Core.Data;
using Sfa.Core.Logging;
using Sfa.Core.Testing;
using Sfa.MyProject.Contexts;
using Sfa.MyProject.Domain;
using Sfa.MyProject.Domain.Builders;

namespace Sfa.MyProject.Testing
{
    /// <summary>
    /// Base class for all domain logic tests.
    /// </summary>
    public abstract class BaseDomainUnitLogicTest : BaseMoqApplicationLogicTest
    {
        #region Life Cycle

        /// <summary>
        /// Ensures that the correct classes will use value equality during testing.
        /// </summary>
        protected override IEnumerable<Assembly> AssembliesWithTypesToPerformFieldValueEqualityOn
        {
            get
            {
                yield return typeof(Root).Assembly;
            }
        }

        protected override void SetUpEachTest()
        {
            base.SetUpEachTest();

            MockRepository = NewMock<IRepository>();
            MockDomainFactory = NewMock<IDomainFactory>();
            MockQueryFactory = NewMock<IQueryFactory>();
            MockNetworkContext = NewMock<INetworkContext>();

            var logger = new MultiLogger(LogggersToUse.ToArray());

            ApplicationContext.Setup(new StaticContextStorage(), logger, MockNetworkContext.Object);
            EnableComparisonLogging();

            DomainContext.Setup(MockRepository.Object,
                                MockDomainFactory.Object,
                                MockQueryFactory.Object);
        }

        protected override void TearDownEachTest()
        {
            DomainContext.TearDown();
            base.TearDownEachTest();
        }

        #endregion Life Cycle


        #region Core Mocks

        public Mock<INetworkContext> MockNetworkContext { get; set; }

        public Mock<IRepository> MockRepository { get; set; }

        public Mock<IDomainFactory> MockDomainFactory { get; set; }

        public Mock<IQueryFactory> MockQueryFactory { get; set; }

        #endregion Core Mocks


        #region Set Expectations
        
        private void SetExpectationsForCreateEntity(BaseEntity entity)
        {
            MockRepository.Expected(o => o.Create(entity))
                .Returns(entity);
        }

        #endregion Set Expections


        #region Builders

        protected RootBuilder NewRoot => new RootBuilder();

        #endregion Builders
    }
}