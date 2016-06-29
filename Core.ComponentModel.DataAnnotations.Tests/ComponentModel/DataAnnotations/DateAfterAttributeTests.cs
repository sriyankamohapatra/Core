using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.ComponentModel.DataAnnotations
{
    [TestClass]
    public class DateAfterAttributeTests : BaseTest
    {
        #region Test Classes

        public class SimplePoco
        {
            public DateTime? MyDateTime { get; set; }

            public string MyString { get; set; }
        }

        #endregion


        #region Constructors

        [TestMethod, TestCategory("Unit")]
        public void FullConstructor()
        {
            // Act
            var componentUnderTest = new DateAfterAttribute("OtherProperty", "ErrorMessage");

            // Assert
            componentUnderTest.OtherOtherPropertyName.ShouldHaveSameValueAs("OtherProperty");
            componentUnderTest.FormatErrorMessage(null).ShouldHaveSameValueAs("ErrorMessage");
        }

        [TestMethod, TestCategory("Unit")]
        public void StandardErrorMessageConstructor()
        {
            // Act
            var componentUnderTest = new DateAfterAttribute("OtherProperty");

            // Assert
            componentUnderTest.OtherOtherPropertyName.ShouldHaveSameValueAs("OtherProperty");
            componentUnderTest.FormatErrorMessage("prop").ShouldHaveSameValueAs("prop should be after OtherProperty");
        }

        #endregion


        #region Validate

        [TestMethod, TestCategory("Unit")]
        public void Validate_Fail_NotMatchedProperty()
        {
            // Arrange
            var componentUnderTest = new DateAfterAttribute("OtherProperty", "ErrorMessage");
            var validationContext = new ValidationContext(new SimplePoco());

            // Act
            try
            {
                componentUnderTest.Validate(null, validationContext);
                Assert.Fail();
            }
            catch (ValidationException validationException)
            {
                validationException.ValidationResult.ErrorMessage.ShouldHaveSameValueAs("Unknown property OtherProperty");
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void Validate_Valid_NullLocalValue()
        {
            // Arrange
            var componentUnderTest = new DateAfterAttribute("MyDateTime", "ErrorMessage");
            var validationContext = new ValidationContext(new SimplePoco());

            // Act
            componentUnderTest.Validate(null, validationContext);
        }

        [TestMethod, TestCategory("Unit")]
        public void Validate_Valid_NullOtherlValue()
        {
            // Arrange
            var componentUnderTest = new DateAfterAttribute("MyDateTime", "ErrorMessage");
            var validationContext = new ValidationContext(new SimplePoco());

            // Act
            componentUnderTest.Validate(new DateTime(2000, 1, 1), validationContext);
        }

        [TestMethod, TestCategory("Unit")]
        public void Validate_Valid_NonDateValueForLocal()
        {
            // Arrange
            var componentUnderTest = new DateAfterAttribute("MyDateTime", "ErrorMessage");
            var validationContext = new ValidationContext(new SimplePoco());

            // Act
            componentUnderTest.Validate("xxx", validationContext);
        }

        [TestMethod, TestCategory("Unit")]
        public void Validate_Valid_NonDateValueForOther()
        {
            // Arrange
            var componentUnderTest = new DateAfterAttribute("MyString", "ErrorMessage");
            var validationContext = new ValidationContext(new SimplePoco {MyString = "xxx"});

            // Act
            componentUnderTest.Validate(new DateTime(2000, 1, 1), validationContext);
        }

        [TestMethod, TestCategory("Unit")]
        public void Validate_InValid_DatesTheSame()
        {
            // Arrange
            var componentUnderTest = new DateAfterAttribute("MyDateTime", "ErrorMessage");
            var validationContext = new ValidationContext(new SimplePoco { MyDateTime = new DateTime(2000, 1, 1) });

            // Act
            try
            {
                componentUnderTest.Validate(new DateTime(2000, 1, 1), validationContext);
                Assert.Fail();
            }
            catch (ValidationException validationException)
            {
                validationException.ValidationResult.ErrorMessage.ShouldHaveSameValueAs("ErrorMessage");
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void Validate_Valid()
        {
            // Arrange
            var componentUnderTest = new DateAfterAttribute("MyDateTime", "ErrorMessage");
            var validationContext = new ValidationContext(new SimplePoco { MyDateTime = new DateTime(2000, 1, 1) });

            // Act
            componentUnderTest.Validate(new DateTime(2000, 1, 10), validationContext);
        }

        #endregion
    }
}