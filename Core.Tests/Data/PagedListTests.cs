using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Data
{
    [TestClass]
    public class PagedListTests : BaseTest
    {
        #region Constructors

        [TestMethod, TestCategory("Unit")]
        public void Constructor_Default()
        {
            // Act
            var actual = new PagedList<string>();

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
        public void Constructor_List()
        {
            // Act
            var actual = new PagedList<string>(new [] {"one", "two", "three"});

            // Assert
            actual.IsPopulated.ShouldHaveSameValueAs(true);
            actual.IsTruncated.ShouldHaveSameValueAs(false);
            actual.TotalNumberOfRecords.ShouldHaveSameValueAs(3);
            actual.TotalNumberOfPages.ShouldHaveSameValueAs(1);
            actual.CurrentPage.ShouldHaveSameValueAs(0);
            actual.PageSize.ShouldHaveSameValueAs(0);

            actual.Count.ShouldHaveSameValueAs(3);
        }

        [TestMethod, TestCategory("Unit")]
        public void Constructor_PagedList()
        {
            // Act
            var actual = new PagedList<string>(new[] { "one", "two", "three" }, 10, 4, 3, 1);

            // Assert
            actual.IsPopulated.ShouldHaveSameValueAs(true);
            actual.IsTruncated.ShouldHaveSameValueAs(true);
            actual.TotalNumberOfRecords.ShouldHaveSameValueAs(10);
            actual.TotalNumberOfPages.ShouldHaveSameValueAs(4);
            actual.CurrentPage.ShouldHaveSameValueAs(1);
            actual.PageSize.ShouldHaveSameValueAs(3);

            actual.Count.ShouldHaveSameValueAs(3);
        }

        #endregion
    }
}