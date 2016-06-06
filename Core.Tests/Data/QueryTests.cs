using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Data
{
    [TestClass]
    public class QueryTests : BaseTest
    {
        #region Test Classes

        public class DefaultQuery : Query<string, string>
        {
            protected override IQueryable<string> Queryable => new[] {"one", "two", "three"}.AsQueryable();

            protected override IQueryable<string> FormLinqQuery(IQueryable<string> query)
            {
                return query;
            }
        }

        public class MatchQuery : Query<string, string>
        {
            private readonly string _toMatch;

            protected override IQueryable<string> Queryable => new[] { "one", "two", "three" }.AsQueryable();

            public MatchQuery(string toMatch)
            {
                _toMatch = toMatch;
            }

            protected override IQueryable<string> FormLinqQuery(IQueryable<string> query)
            {
                return query.Where(o => o == _toMatch);
            }
        }

        #endregion


        #region GetResultList


        [TestMethod, TestCategory("Unit")]
        public void GetResultList_Page1()
        {
            // Arrange
            var componentUnderTest = new DefaultQuery();
            var expected = new ResultList<string>(new[] { "one", "two"}, true, 3, 2, 2, 0);

            // Act
            var actual = componentUnderTest.GetResultList(0, 2);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }


        [TestMethod, TestCategory("Unit")]
        public void GetResultList_Page2()
        {
            // Arrange
            var componentUnderTest = new DefaultQuery();
            var expected = new ResultList<string>(new[] { "three" }, true, 3, 2, 2, 1);

            // Act
            var actual = componentUnderTest.GetResultList(1, 2);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        #endregion


        #region GetList

        [TestMethod, TestCategory("Unit")]
        public void GetList()
        {
            // Arrange
            var componentUnderTest = new DefaultQuery();

            // Act
            var actual = componentUnderTest.GetList();

            // Assert
            actual.ShouldHaveSameValueAs(new[] {"one", "two", "three"});
        }

        #endregion


        #region GetCount

        [TestMethod, TestCategory("Unit")]
        public void GetCount()
        {
            // Arrange
            var componentUnderTest = new DefaultQuery();

            // Act
            var actual = componentUnderTest.GetCount();

            // Assert
            actual.ShouldHaveSameValueAs(3);
        }

        #endregion


        #region GetExists


        [TestMethod, TestCategory("Unit")]
        public void GetExists_Exists()
        {
            // Arrange
            var componentUnderTest = new MatchQuery("two");

            // Act
            var actual = componentUnderTest.GetExists();

            // Assert
            actual.ShouldHaveSameValueAs(true);
        }


        [TestMethod, TestCategory("Unit")]
        public void GetExists_DoesNotExists()
        {
            // Arrange
            var componentUnderTest = new MatchQuery("four");

            // Act
            var actual = componentUnderTest.GetExists();

            // Assert
            actual.ShouldHaveSameValueAs(false);
        }

        #endregion
    }
}