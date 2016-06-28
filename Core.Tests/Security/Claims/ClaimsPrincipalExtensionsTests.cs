using System.Linq;
using System.Security.Claims;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Security.Claims
{
    [TestClass]
    public class ClaimsPrincipalExtensionsTests : BaseTest
    {
        #region AddClaim

        [TestMethod, TestCategory("Unit")]
        public void AddClaim()
        {
            // Arrange
            var componentUnderTest = new ClaimsPrincipal(new [] { new ClaimsIdentity()});

            // Act
            componentUnderTest.AddClaim("myType", "value1");

            // Assert
            var claim = componentUnderTest.Claims.First();
            claim.Type.ShouldHaveSameValueAs("myType");
            claim.Value.ShouldHaveSameValueAs("value1");
        }

        #endregion


        #region RemoveClaimsOfType


        [TestMethod, TestCategory("Unit")]
        public void RemoveClaimsOfType()
        {
            // Arrange
            var identity1 = new ClaimsIdentity();
            var identity2 = new ClaimsIdentity();

            identity1.AddClaim(new Claim("type1", "value11"));
            identity1.AddClaim(new Claim("type2", "value12"));
            identity1.AddClaim(new Claim("type3", "value13"));

            identity2.AddClaim(new Claim("type1", "value21"));
            identity2.AddClaim(new Claim("type2", "value22"));
            identity2.AddClaim(new Claim("type3", "value23"));

            var componentUnderTest = new ClaimsPrincipal(new[] { identity1, identity2 });

            // Act
            componentUnderTest.RemoveClaimsOfType("type2");

            // Assert
            componentUnderTest.Claims.Count(o => o.Type == "type1").ShouldHaveSameValueAs(2);
            componentUnderTest.Claims.Count(o => o.Type == "type2").ShouldHaveSameValueAs(0);
            componentUnderTest.Claims.Count(o => o.Type == "type3").ShouldHaveSameValueAs(2);
        }


        #endregion
    }
}