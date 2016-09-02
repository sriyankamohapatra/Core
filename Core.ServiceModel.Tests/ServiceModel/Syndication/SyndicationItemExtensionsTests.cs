﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.ServiceModel.Syndication;
using Sfa.Core.Testing;
using System;
using System.ServiceModel.Syndication;

namespace Core.ServiceModel.Tests.ServiceModel.Syndication
{
    [TestClass]
    public class SyndicationItemExtensionsTests : BaseTest
    {
        [TestMethod, TestCategory("Unit")]
        public void IdAsGuid_IdWithUuid()
        {
            // Arrange
            var syndicationItem = new SyndicationItem { Id = "uuid:fdcfcfcd-764a-46a7-b8cf-5b87d15906fa"};
            var expected = new Guid("fdcfcfcd-764a-46a7-b8cf-5b87d15906fa");

            // Act
            var actual = syndicationItem.IdAsGuid();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("Unit")]
        public void IdAsGuid_IdWithoutUuid()
        {
            // Arrange
            var syndicationItem = new SyndicationItem { Id = "fdcfcfcd-764a-46a7-b8cf-5b87d15906fa" };
            var expected = new Guid("fdcfcfcd-764a-46a7-b8cf-5b87d15906fa");

            // Act
            var actual = syndicationItem.IdAsGuid();

            // Assert
            Assert.AreEqual(expected, actual);
        }

    }
}
