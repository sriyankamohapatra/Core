using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Exceptions;
using Sfa.Core.Testing;

namespace Sfa.Core.Data
{
    [TestClass]
    public class QueryTests : BaseTest
    {
        #region Life Cycle

        protected override IEnumerable<Assembly> AssembliesWithTypesToPerformFieldValueEqualityOn
        {
            get { yield return typeof(QueryTests).Assembly; }
        }

        #endregion


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

        public class SimplePocoQuery<TReturn> : Query<TReturn, SimplePoco>
        {
            protected override IQueryable<SimplePoco> Queryable => new[]
            {
                new SimplePoco
                {
                    Id = 1,
                    Value1 = "v1.1",
                    Value2 = "v1.2",
                    ChildPoco = new SimplePoco
                    {
                        Id = 2,
                        Value1 = "v2.1"
                    }
                },
                new SimplePoco
                {
                    Id = 3,
                    Value1 = "v3.1",
                    Value2 = "v3.2",
                    ChildPoco = new SimplePoco
                    {
                        Id = 4,
                        Value1 = "v4.1"
                    }
                },
            }.AsQueryable();

            protected override IQueryable<SimplePoco> FormLinqQuery(IQueryable<SimplePoco> query)
            {
                return query;
            }
        }

        public class SimplePoco
        {
            public int Id { get; set; }

            public string Value1 { get; set; }

            public string Value2 { get; set; }

            public SimplePoco ChildPoco { get; set; }
        }

        public class MissingImplementationProjection : IProjection
        {
            public string Value1 { get; set; }

            public string ChildValue1 { get; set; }
        }

        public class SimplePocoProjection : IProjection
        {
            public string Value1 { get; set; }

            public string ChildValue1 { get; set; }

            public static IQueryable<SimplePocoProjection> AddProjection(IQueryable<SimplePoco> query)
            {
                return from simplePoco in query
                    select new SimplePocoProjection
                    {
                        Value1 = simplePoco.Value1,
                        ChildValue1 = simplePoco.ChildPoco.Value1
                    };
            }
        }

        #endregion


        #region GetResultList


        [TestMethod, TestCategory("Unit")]
        public void GetResultList_NotPaged()
        {
            // Arrange
            var componentUnderTest = new DefaultQuery();
            var expected = new ResultList<string>(new[] { "one", "two", "three" }, true, 3, 0, 0, 0);

            // Act
            var actual = componentUnderTest.GetResultList(0, 0);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
            componentUnderTest.PageNumber.ShouldHaveSameValueAs(0);
            componentUnderTest.PageSize.ShouldHaveSameValueAs(0);
        }


        [TestMethod, TestCategory("Unit")]
        public void GetResultList_Page1_NoResults()
        {
            // Arrange
            var componentUnderTest = new MatchQuery("xxx");
            var expected = new ResultList<string>(new string[0], true, 0, 0, 2, 0);

            // Act
            var actual = componentUnderTest.GetResultList(0, 2);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
            componentUnderTest.PageNumber.ShouldHaveSameValueAs(0);
            componentUnderTest.PageSize.ShouldHaveSameValueAs(2);
        }


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
            componentUnderTest.PageNumber.ShouldHaveSameValueAs(0);
            componentUnderTest.PageSize.ShouldHaveSameValueAs(2);
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
            componentUnderTest.PageNumber.ShouldHaveSameValueAs(1);
            componentUnderTest.PageSize.ShouldHaveSameValueAs(2);
        }


        [TestMethod, TestCategory("Unit")]
        public void GetResultList_Page10of2()
        {
            // Arrange
            var componentUnderTest = new DefaultQuery();
            var expected = new ResultList<string>(new[] { "three" }, true, 3, 2, 2, 1);

            // Act
            var actual = componentUnderTest.GetResultList(10, 2);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
            componentUnderTest.PageNumber.ShouldHaveSameValueAs(1);
            componentUnderTest.PageSize.ShouldHaveSameValueAs(2);
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


        #region GetSingle


        [TestMethod, TestCategory("Unit")]
        public void GetSingle_Exists()
        {
            // Arrange
            var componentUnderTest = new MatchQuery("two");

            // Act
            var actual = componentUnderTest.GetSingle();

            // Assert
            actual.ShouldHaveSameValueAs("two");
        }


        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(MissingEntityException))]
        public void GetSingle_DoesNotExists()
        {
            // Arrange
            var componentUnderTest = new MatchQuery("four");

            // Act
            componentUnderTest.GetSingle();
        }

        #endregion


        #region GetSingleIfExists


        [TestMethod, TestCategory("Unit")]
        public void GetSingleIfExists_Exists()
        {
            // Arrange
            var componentUnderTest = new MatchQuery("two");

            // Act
            var actual = componentUnderTest.GetSingleIfExists();

            // Assert
            actual.ShouldHaveSameValueAs("two");
        }


        [TestMethod, TestCategory("Unit")]
        public void GetSingleIfExists_DoesNotExists()
        {
            // Arrange
            var componentUnderTest = new MatchQuery("four");

            // Act
            var actual = componentUnderTest.GetSingleIfExists();

            // Assert
            Assert.IsNull(actual);
        }

        #endregion


        #region GetAnyItemsExist


        [TestMethod, TestCategory("Unit")]
        public void GetAnyItemsExist_Exists()
        {
            // Arrange
            var componentUnderTest = new MatchQuery("two");

            // Act
            var actual = componentUnderTest.GetAnyItemsExist();

            // Assert
            actual.ShouldHaveSameValueAs(true);
        }


        [TestMethod, TestCategory("Unit")]
        public void GetAnyItemsExist_DoesNotExists()
        {
            // Arrange
            var componentUnderTest = new MatchQuery("four");

            // Act
            var actual = componentUnderTest.GetAnyItemsExist();

            // Assert
            actual.ShouldHaveSameValueAs(false);
        }

        #endregion


        #region Projections
        
        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Projections_MissingImplementation()
        {
            // Arrange
            var componentUnderTest = new SimplePocoQuery<MissingImplementationProjection>();

            // Act
            componentUnderTest.GetList();
        }


        [TestMethod, TestCategory("Unit")]
        public void Projections()
        {
            // Arrange
            var componentUnderTest = new SimplePocoQuery<SimplePocoProjection>();

            // Act
            var actual = componentUnderTest.GetList();

            // Assert
            actual.ShouldHaveSameValueAs(new []
            {
                new SimplePocoProjection
                {
                    Value1 = "v1.1",
                    ChildValue1 = "v2.1"
                },
                new SimplePocoProjection
                {
                    Value1 = "v3.1",
                    ChildValue1 = "v4.1"
                },
            });
        }


        [TestMethod, TestCategory("Unit")]
        public void Paged_Projections()
        {
            // Arrange
            var componentUnderTest = new SimplePocoQuery<SimplePocoProjection>();

            // Act
            var actual = componentUnderTest.GetResultList(0, 1);

            // Assert
            actual.ShouldHaveSameValueAs(new ResultList<SimplePocoProjection>(new[]
            {
                new SimplePocoProjection
                {
                    Value1 = "v1.1",
                    ChildValue1 = "v2.1"
                }
            }));
        }

        #endregion
    }
}