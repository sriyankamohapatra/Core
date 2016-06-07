using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Data
{
    [TestClass]
    public class ResultListTests : BaseTest
    {
        #region Constructors

        [TestMethod, TestCategory("Unit")]
        public void Constructor_Default()
        {
            // Act
            var actual = new ResultList<string>();

            // Assert
            actual.IsPopulated.ShouldHaveSameValueAs(false);
            actual.IsTruncated.ShouldHaveSameValueAs(false);
            actual.TotalNumberOfRecords.ShouldHaveSameValueAs(0);
            actual.TotalNumberOfPages.ShouldHaveSameValueAs(0);
            actual.CurrentPage.ShouldHaveSameValueAs(0);
            actual.PageSize.ShouldHaveSameValueAs(0);

            actual.Count.ShouldHaveSameValueAs(0);
        }

        [TestMethod, TestCategory("Unit")]
        public void Constructor_Data()
        {
            // Act
            var actual = new ResultList<string>(new [] {"one", "two", "three"});

            // Assert
            actual.IsPopulated.ShouldHaveSameValueAs(true);
            actual.IsTruncated.ShouldHaveSameValueAs(false);
            actual.TotalNumberOfRecords.ShouldHaveSameValueAs(0);
            actual.TotalNumberOfPages.ShouldHaveSameValueAs(0);
            actual.CurrentPage.ShouldHaveSameValueAs(0);
            actual.PageSize.ShouldHaveSameValueAs(0);

            actual.Count.ShouldHaveSameValueAs(3);
        }

        [TestMethod, TestCategory("Unit")]
        public void Constructor_TruncatedData()
        {
            // Act
            var actual = new ResultList<string>(new[] { "one", "two", "three" }, true);

            // Assert
            actual.IsPopulated.ShouldHaveSameValueAs(true);
            actual.IsTruncated.ShouldHaveSameValueAs(true);
            actual.TotalNumberOfRecords.ShouldHaveSameValueAs(0);
            actual.TotalNumberOfPages.ShouldHaveSameValueAs(0);
            actual.CurrentPage.ShouldHaveSameValueAs(0);
            actual.PageSize.ShouldHaveSameValueAs(0);

            actual.Count.ShouldHaveSameValueAs(3);
        }

        [TestMethod, TestCategory("Unit")]
        public void Constructor_TruncatedPopulatedData()
        {
            // Act
            var actual = new ResultList<string>(new[] { "one", "two", "three" }, true, true);

            // Assert
            actual.IsPopulated.ShouldHaveSameValueAs(true);
            actual.IsTruncated.ShouldHaveSameValueAs(true);
            actual.TotalNumberOfRecords.ShouldHaveSameValueAs(0);
            actual.TotalNumberOfPages.ShouldHaveSameValueAs(0);
            actual.CurrentPage.ShouldHaveSameValueAs(0);
            actual.PageSize.ShouldHaveSameValueAs(0);

            actual.Count.ShouldHaveSameValueAs(3);
        }

        [TestMethod, TestCategory("Unit")]
        public void Constructor_Full()
        {
            // Act
            var actual = new ResultList<string>(new[] { "one", "two", "three" }, true, 1, 2, 3, 4);

            // Assert
            actual.IsPopulated.ShouldHaveSameValueAs(true);
            actual.IsTruncated.ShouldHaveSameValueAs(true);
            actual.TotalNumberOfRecords.ShouldHaveSameValueAs(1);
            actual.TotalNumberOfPages.ShouldHaveSameValueAs(2);
            actual.CurrentPage.ShouldHaveSameValueAs(4);
            actual.PageSize.ShouldHaveSameValueAs(3);

            actual.Count.ShouldHaveSameValueAs(3);
        }

        [TestMethod, TestCategory("Unit")]
        public void Constructor_Object()
        {
            // Act
            var actual = new ResultList<string>("one");

            // Assert
            actual.IsPopulated.ShouldHaveSameValueAs(true);
            actual.IsTruncated.ShouldHaveSameValueAs(false);
            actual.TotalNumberOfRecords.ShouldHaveSameValueAs(0);
            actual.TotalNumberOfPages.ShouldHaveSameValueAs(0);
            actual.CurrentPage.ShouldHaveSameValueAs(0);
            actual.PageSize.ShouldHaveSameValueAs(0);

            actual.Count.ShouldHaveSameValueAs(1);
        }

        #endregion
    }
}