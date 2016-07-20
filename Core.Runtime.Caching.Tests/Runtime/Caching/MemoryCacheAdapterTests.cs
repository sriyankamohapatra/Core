using System;
using System.Runtime.Caching;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Context;
using Sfa.Core.Testing;

namespace Sfa.Core.Runtime.Caching
{
    [TestClass]
    public class MemoryCacheAdapterTests : BaseTest
    {
        #region ContainsKey

        [TestMethod, TestCategory("Unit")]
        public void ContainsKey_True()
        {
            // Arrange
            MemoryCache.Default.Add("ContainsKey_True", "test value", new CacheItemPolicy());
            var componentUnderTest = new MemoryCacheAdapter();

            // Act
            var actual = componentUnderTest.ContainsKey("ContainsKey_True");

            // Assert
            actual.ShouldHaveSameValueAs(true);
        }

        [TestMethod, TestCategory("Unit")]
        public void ContainsKey_False()
        {
            // Arrange
            MemoryCache.Default.Remove("ContainsKey_False");
            var componentUnderTest = new MemoryCacheAdapter();

            // Act
            var actual = componentUnderTest.ContainsKey("ContainsKey_False");

            // Assert
            actual.ShouldHaveSameValueAs(false);
        }

        #endregion


        #region Delete

        [TestMethod, TestCategory("Unit")]
        public void Delete_Exists()
        {
            // Arrange
            MemoryCache.Default.Add("Delete_Exists", "some value", new CacheItemPolicy());
            var componentUnderTest = new MemoryCacheAdapter();

            // Act
            componentUnderTest.Delete("Delete_Exists");

            // Assert
            MemoryCache.Default.Contains("Delete_Exists").ShouldHaveSameValueAs(false);
        }

        [TestMethod, TestCategory("Unit")]
        public void Delete_NotExists()
        {
            // Arrange
            MemoryCache.Default.Remove("Delete_NotExists");
            var componentUnderTest = new MemoryCacheAdapter();

            // Act
            componentUnderTest.Delete("Delete_NotExists");

            // Assert
            MemoryCache.Default.Contains("Delete_NotExists").ShouldHaveSameValueAs(false);
        }

        #endregion


        #region Get


        [TestMethod, TestCategory("Unit")]
        public void Get_Exists()
        {
            // Arrange
            MemoryCache.Default.Add("Get_Exists", "some value", new CacheItemPolicy());
            var componentUnderTest = new MemoryCacheAdapter();

            // Act
            var actual = componentUnderTest.Get<string>("Get_Exists");

            // Assert
            actual.ShouldHaveSameValueAs("some value");
        }

        [TestMethod, TestCategory("Unit")]
        public void Get_NotExists()
        {
            // Arrange
            MemoryCache.Default.Remove("Get_NotExists");
            var componentUnderTest = new MemoryCacheAdapter();

            // Act
            var actual = componentUnderTest.Get<string>("Get_NotExists");

            // Assert
            actual.ShouldHaveSameValueAs(null);
        }

        #endregion


        #region GetOrAddAndGet


        [TestMethod, TestCategory("Unit")]
        public void GetOrAddAndGet_Exists()
        {
            // Arrange
            MemoryCache.Default.Add("GetOrAddAndGet_Exists", "value1", new CacheItemPolicy());
            var componentUnderTest = new MemoryCacheAdapter();

            // Act
            var actual = componentUnderTest.GetOrAddAndGet("GetOrAddAndGet_Exists", () => "value2");

            // Assert
            actual.ShouldHaveSameValueAs("value1");
            MemoryCache.Default.Get("GetOrAddAndGet_Exists").ShouldHaveSameValueAs("value1");
        }

        [TestMethod, TestCategory("Unit")]
        public void GetOrAddAndGet_NotExists()
        {
            // Arrange
            MemoryCache.Default.Remove("GetOrAddAndGet_NotExists");
            var componentUnderTest = new MemoryCacheAdapter();

            // Act
            var actual = componentUnderTest.GetOrAddAndGet("GetOrAddAndGet_NotExists", () => "value2");

            // Assert
            actual.ShouldHaveSameValueAs("value2");
            MemoryCache.Default.Get("GetOrAddAndGet_NotExists").ShouldHaveSameValueAs("value2");
        }


        #endregion


        #region GetOrAddAndGetSlidingExpiration


        [TestMethod, TestCategory("Unit")]
        public void GetOrAddAndGetSlidingExpiration_Exists()
        {
            // Arrange
            MemoryCache.Default.Add("GetOrAddAndGetSlidingExpiration_Exists", "value1", new CacheItemPolicy());
            var componentUnderTest = new MemoryCacheAdapter();

            // Act
            var actual = componentUnderTest.GetOrAddAndGetSlidingExpiration("GetOrAddAndGetSlidingExpiration_Exists", TimeSpan.FromMinutes(1), () => "value2");

            // Assert
            actual.ShouldHaveSameValueAs("value1");
            MemoryCache.Default.Get("GetOrAddAndGetSlidingExpiration_Exists").ShouldHaveSameValueAs("value1");
        }

        [TestMethod, TestCategory("Unit")]
        public void GetOrAddAndGetSlidingExpiration_NotExists()
        {
            // Arrange
            MemoryCache.Default.Remove("GetOrAddAndGetSlidingExpiration_NotExists");
            var componentUnderTest = new MemoryCacheAdapter();

            // Act
            var actual = componentUnderTest.GetOrAddAndGetSlidingExpiration("GetOrAddAndGetSlidingExpiration_NotExists", TimeSpan.FromMinutes(1), () => "value2");

            // Assert
            actual.ShouldHaveSameValueAs("value2");
            MemoryCache.Default.Get("GetOrAddAndGetSlidingExpiration_NotExists").ShouldHaveSameValueAs("value2");
        }

        [TestMethod, TestCategory("Unit")]
        public void GetOrAddAndGetSlidingExpiration_NotExists_RenewAndThenTimeout()
        {
            // Arrange
            const string key = "GetOrAddAndGetSlidingExpiration_NotExists_RenewAndThenTimeout";
            MemoryCache.Default.Remove(key);
            var componentUnderTest = new MemoryCacheAdapter();

            // Act
            var actual = componentUnderTest.GetOrAddAndGetSlidingExpiration(key, TimeSpan.FromSeconds(2), () => "value2");

            // Assert
            actual.ShouldHaveSameValueAs("value2");
            MemoryCache.Default.Get(key).ShouldHaveSameValueAs("value2");

            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(1000);
                componentUnderTest.Get<string>(key).ShouldHaveSameValueAs("value2");
            }
            Thread.Sleep(2100);
            componentUnderTest.Get<string>(key).ShouldHaveSameValueAs(null);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetOrAddAndGetSlidingExpiration_NotExists_Timeout()
        {
            // Arrange
            MemoryCache.Default.Remove("GetOrAddAndGetSlidingExpiration_NotExists_Timeout");
            var componentUnderTest = new MemoryCacheAdapter();

            // Act
            var actual = componentUnderTest.GetOrAddAndGetSlidingExpiration("GetOrAddAndGetSlidingExpiration_NotExists_Timeout", TimeSpan.FromSeconds(2), () => "value2");

            // Assert
            actual.ShouldHaveSameValueAs("value2");
            MemoryCache.Default.Get("GetOrAddAndGetSlidingExpiration_NotExists_Timeout").ShouldHaveSameValueAs("value2");

            Thread.Sleep(2100);
            componentUnderTest.Get<string>("GetOrAddAndGetSlidingExpiration_NotExists_Timeout").ShouldHaveSameValueAs(null);
        }


        #endregion


        #region GetOrAddAndGetExactExpiration


        [TestMethod, TestCategory("Unit")]
        public void GetOrAddAndGetExactExpiration_Exists()
        {
            // Arrange
            MemoryCache.Default.Add("GetOrAddAndGetExactExpiration_Exists", "value1", new CacheItemPolicy());
            var componentUnderTest = new MemoryCacheAdapter();

            // Act
            var actual = componentUnderTest.GetOrAddAndGetExactExpiration("GetOrAddAndGetExactExpiration_Exists", ApplicationContext.NetworkContext.CurrentDateTime.AddSeconds(2), () => "value2");

            // Assert
            actual.ShouldHaveSameValueAs("value1");
            MemoryCache.Default.Get("GetOrAddAndGetExactExpiration_Exists").ShouldHaveSameValueAs("value1");
        }

        [TestMethod, TestCategory("Unit")]
        public void GetOrAddAndGetExactExpiration_NotExists()
        {
            // Arrange
            MemoryCache.Default.Remove("GetOrAddAndGetExactExpiration_NotExists");
            var componentUnderTest = new MemoryCacheAdapter();

            // Act
            var actual = componentUnderTest.GetOrAddAndGetExactExpiration("GetOrAddAndGetExactExpiration_NotExists", ApplicationContext.NetworkContext.CurrentDateTime.AddSeconds(2), () => "value2");

            // Assert
            actual.ShouldHaveSameValueAs("value2");
            MemoryCache.Default.Get("GetOrAddAndGetExactExpiration_NotExists").ShouldHaveSameValueAs("value2");
        }

        [TestMethod, TestCategory("Unit")]
        public void GetOrAddAndGetExactExpiration_NotExists_Timeout()
        {
            // Arrange
            MemoryCache.Default.Remove("GetOrAddAndGetExactExpiration_NotExists_Timeout");
            var componentUnderTest = new MemoryCacheAdapter();

            // Act
            var actual = componentUnderTest.GetOrAddAndGetExactExpiration("GetOrAddAndGetExactExpiration_NotExists_Timeout", ApplicationContext.NetworkContext.CurrentDateTime.AddSeconds(2), () => "value2");

            // Assert
            actual.ShouldHaveSameValueAs("value2");
            MemoryCache.Default.Get("GetOrAddAndGetExactExpiration_NotExists_Timeout").ShouldHaveSameValueAs("value2");

            for (var i = 0; i < 3; i++)
            {
                Thread.Sleep(500);
                componentUnderTest.Get<string>("GetOrAddAndGetExactExpiration_NotExists_Timeout").ShouldHaveSameValueAs("value2");
            }
            Thread.Sleep(501);
            componentUnderTest.Get<string>("GetOrAddAndGetExactExpiration_NotExists_Timeout").ShouldHaveSameValueAs(null);
        }


        #endregion


        #region Add


        [TestMethod, TestCategory("Unit")]
        public void Add_Exists()
        {
            // Arrange
            MemoryCache.Default.Add("Add_Exists", "value1", new CacheItemPolicy());
            var componentUnderTest = new MemoryCacheAdapter();

            // Act
            componentUnderTest.Add("Add_Exists", "value2");

            // Assert
            MemoryCache.Default.Get("Add_Exists").ShouldHaveSameValueAs("value2");
        }

        [TestMethod, TestCategory("Unit")]
        public void Add_NotExists()
        {
            // Arrange
            MemoryCache.Default.Remove("Add_NotExists");
            var componentUnderTest = new MemoryCacheAdapter();

            // Act
            componentUnderTest.Add("Add_NotExists", "value2");

            // Assert
            MemoryCache.Default.Get("Add_NotExists").ShouldHaveSameValueAs("value2");
        }


        #endregion


        #region AddSlidingExpiration


        [TestMethod, TestCategory("Unit")]
        public void AddSlidingExpiration_Exists()
        {
            // Arrange
            MemoryCache.Default.Add("AddSlidingExpiration_Exists", "value1", new CacheItemPolicy());
            var componentUnderTest = new MemoryCacheAdapter();

            // Act
            componentUnderTest.AddSlidingExpiration("AddSlidingExpiration_Exists", "value2", TimeSpan.FromMinutes(1));

            // Assert
            MemoryCache.Default.Get("AddSlidingExpiration_Exists").ShouldHaveSameValueAs("value2");
        }

        [TestMethod, TestCategory("Unit")]
        public void AddSlidingExpiration_NotExists()
        {
            // Arrange
            MemoryCache.Default.Remove("AddSlidingExpiration_NotExists");
            var componentUnderTest = new MemoryCacheAdapter();

            // Act
            componentUnderTest.AddSlidingExpiration("AddSlidingExpiration_NotExists", "value2", TimeSpan.FromMinutes(1));

            // Assert
            MemoryCache.Default.Get("AddSlidingExpiration_NotExists").ShouldHaveSameValueAs("value2");
        }

        [TestMethod, TestCategory("Unit")]
        public void AddSlidingExpiration_NotExists_RenewAndThenTimeout()
        {
            // Arrange
            const string key = "AddSlidingExpiration_NotExists_RenewAndThenTimeout";
            MemoryCache.Default.Remove(key);
            var componentUnderTest = new MemoryCacheAdapter();

            // Act
            componentUnderTest.AddSlidingExpiration(key, "value2", TimeSpan.FromSeconds(2));

            // Assert
            componentUnderTest.Get<string>(key).ShouldHaveSameValueAs("value2");

            for (var i = 0; i < 5; i++)
            {
                Thread.Sleep(500);
                componentUnderTest.Get<string>(key).ShouldHaveSameValueAs("value2");
            }
            Thread.Sleep(2100);
            componentUnderTest.Get<string>(key).ShouldHaveSameValueAs(null);
        }

        [TestMethod, TestCategory("Unit")]
        public void AddSlidingExpiration_NotExists_Timeout()
        {
            // Arrange
            MemoryCache.Default.Remove("AddSlidingExpiration_NotExists_Timeout");
            var componentUnderTest = new MemoryCacheAdapter();

            // Act
            componentUnderTest.AddSlidingExpiration("AddSlidingExpiration_NotExists_Timeout", "value2", TimeSpan.FromSeconds(2));

            // Assert
            MemoryCache.Default.Get("AddSlidingExpiration_NotExists_Timeout").ShouldHaveSameValueAs("value2");

            Thread.Sleep(2100);
            componentUnderTest.Get<string>("AddSlidingExpiration_NotExists_Timeout").ShouldHaveSameValueAs(null);
        }


        #endregion


        #region AddExactExpiration


        [TestMethod, TestCategory("Unit")]
        public void AddExactExpiration_Exists()
        {
            // Arrange
            MemoryCache.Default.Add("AddExactExpiration_Exists", "value1", new CacheItemPolicy());
            var componentUnderTest = new MemoryCacheAdapter();

            // Act
            componentUnderTest.AddExactExpiration("AddExactExpiration_Exists", "value2", ApplicationContext.NetworkContext.CurrentDateTime.AddSeconds(2));

            // Assert
            MemoryCache.Default.Get("AddExactExpiration_Exists").ShouldHaveSameValueAs("value2");
        }

        [TestMethod, TestCategory("Unit")]
        public void AddExactExpiration_NotExists()
        {
            // Arrange
            MemoryCache.Default.Remove("AddExactExpiration_NotExists");
            var componentUnderTest = new MemoryCacheAdapter();

            // Act
            componentUnderTest.AddExactExpiration("AddExactExpiration_NotExists", "value2", ApplicationContext.NetworkContext.CurrentDateTime.AddSeconds(2));

            // Assert
            MemoryCache.Default.Get("AddExactExpiration_NotExists").ShouldHaveSameValueAs("value2");
        }

        [TestMethod, TestCategory("Unit")]
        public void AddExactExpiration_NotExists_Timeout()
        {
            // Arrange
            MemoryCache.Default.Remove("AddExactExpiration_NotExists_Timeout");
            var componentUnderTest = new MemoryCacheAdapter();

            // Act
            componentUnderTest.AddExactExpiration("AddExactExpiration_NotExists_Timeout", "value2", ApplicationContext.NetworkContext.CurrentDateTime.AddSeconds(2));

            // Assert
            MemoryCache.Default.Get("AddExactExpiration_NotExists_Timeout").ShouldHaveSameValueAs("value2");

            for (var i = 0; i < 3; i++)
            {
                Thread.Sleep(500);
                componentUnderTest.Get<string>("AddExactExpiration_NotExists_Timeout").ShouldHaveSameValueAs("value2");
            }
            Thread.Sleep(500);
            componentUnderTest.Get<string>("AddExactExpiration_NotExists_Timeout").ShouldHaveSameValueAs(null);
        }


        #endregion
    }
}
