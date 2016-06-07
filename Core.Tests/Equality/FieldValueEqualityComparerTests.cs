using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Equality
{
    /// <summary>
    /// REMEMBER must use Assert.<xxx> here!!
    /// </summary>
    [TestClass]
    public class FieldValueEqualityComparerTests : BaseTest
    {
        #region TestClasses

        public class SimplePoco
        {
            [NoValueCompare]
            private string _myIgnoredString;

            public int MyInt { get; set; }

            public string MyString { get; set; }

            public DateTime MyDateTime { get; set; }

            public string MyIgnoredString
            {
                get { return _myIgnoredString; }
                set { _myIgnoredString = value; }
            }
        }

        public class ComplexPoco
        {
            public int MyInt { get; set; }

            public string[] MyStrings { get; set; }

            public SimplePoco[] SimplePocoArray { get; set; }

            public Dictionary<string, SimplePoco> SimplePocoDictionary = new Dictionary<string, SimplePoco>();
        }


        public class CircularReference
        {
            public CircularReference Child { get; set; }

            public CircularReference Parent { get; set; }
            
            public string MyString { get; set; }

            public CircularReference OtheReference { get; set; }
        }

        #endregion


        #region GetHashCode

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(NotImplementedException))]
        public void GetHashCode_Test()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();

            // Act
            componentUnderTest.GetHashCode(new object());
        }

        #endregion


        #region Simple Default Equals


        [TestMethod, TestCategory("Unit")]
        public void Equals_True_Strings()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();

            // Act
            var actual = componentUnderTest.Equals("a", "a");

            // Assert
            Assert.AreEqual(true, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_False_Strings()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();

            // Act
            var actual = componentUnderTest.Equals("a", "b");

            // Assert
            Assert.AreEqual(false, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_True_Nulls()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();

            // Act
            var actual = componentUnderTest.Equals(null, null);

            // Assert
            Assert.AreEqual(true, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_False_Object_And_Null()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();

            // Act
            var actual = componentUnderTest.Equals(new object(), (object)null);

            // Assert
            Assert.AreEqual(false, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_True_ReferenceEquals()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            var obj = new object();

            // Act
            var actual = componentUnderTest.Equals(obj, obj);

            // Assert
            Assert.AreEqual(true, actual);
        }

        #endregion


        #region SimplePoco

        [TestMethod, TestCategory("Unit")]
        public void Equals_False_SimplePoco_NotSetTheAssembly()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new Assembly[] { });

            // Act
            var actual = componentUnderTest.Equals(new SimplePoco(), new SimplePoco());

            // Assert
            Assert.AreEqual(false, actual);
        }

        [TestMethod, TestCategory("Unit")]
        public void Equals_True_SimplePoco()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            // Act
            var actual = componentUnderTest.Equals(new SimplePoco(), new SimplePoco());

            // Assert
            Assert.AreEqual(true, actual);
        }

        [TestMethod, TestCategory("Unit")]
        public void Equals_True_SimplePoco_ValuesSet()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            // Act
            var actual = componentUnderTest.Equals(new SimplePoco
            {
                MyInt = 1,
                MyString = "test",
                MyIgnoredString = "xxx",
                MyDateTime = new DateTime(2000, 1, 1)
            }, new SimplePoco
            {
                MyInt = 1,
                MyString = "test",
                MyIgnoredString = "xxx",
                MyDateTime = new DateTime(2000, 1, 1)
            });

            // Assert
            Assert.AreEqual(true, actual);
        }

        [TestMethod, TestCategory("Unit")]
        public void Equals_False_SimplePoco_ValuesSet()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            // Act
            var actual = componentUnderTest.Equals(new SimplePoco
            {
                MyInt = 1,
                MyString = "test",
                MyIgnoredString = "xxx",
                MyDateTime = new DateTime(2000, 1, 1)
            }, new SimplePoco
            {
                MyInt = 1,
                MyString = "test other",
                MyIgnoredString = "xxx",
                MyDateTime = new DateTime(2000, 1, 1)
            });

            // Assert
            Assert.AreEqual(false, actual);
        }

        [TestMethod, TestCategory("Unit")]
        public void Equals_True_SimplePoco_ValuesSet_Use_NoValueCompareAttribute()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            // Act
            var actual = componentUnderTest.Equals(new SimplePoco
            {
                MyInt = 1,
                MyString = "test",
                MyIgnoredString = "some value",
                MyDateTime = new DateTime(2000, 1, 1)
            }, new SimplePoco
            {
                MyInt = 1,
                MyString = "test",
                MyIgnoredString = "some different value",
                MyDateTime = new DateTime(2000, 1, 1)
            });

            // Assert
            Assert.AreEqual(true, actual);
        }

        #endregion


        #region ComplexPoco
        

        [TestMethod, TestCategory("Unit")]
        public void Equals_True_ComplexPoco()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });
            var simplePoco = new SimplePoco {MyInt = 23};

            // Act
            var actual = componentUnderTest.Equals(new ComplexPoco
            {
                MyInt = 1,
                MyStrings = new[] {"1", "2"},
                SimplePocoArray = new[] {new SimplePoco(), simplePoco },
                SimplePocoDictionary = new Dictionary<string, SimplePoco>
                {
                    {"1", new SimplePoco {MyString = "test"}},
                    {"2", simplePoco}
                }
            }, new ComplexPoco
            {
                MyInt = 1,
                MyStrings = new[] {"1", "2"},
                SimplePocoArray = new[] { new SimplePoco(), simplePoco },
                SimplePocoDictionary = new Dictionary<string, SimplePoco>
                {
                    {"1", new SimplePoco {MyString = "test"}},
                    {"2", simplePoco}
                }
            });

            // Assert
            Assert.AreEqual(true, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_False_ComplexPoco_ArrayNotEqual()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            // Act
            var actual = componentUnderTest.Equals(new ComplexPoco
            {
                MyStrings = new[] { "1", "2" }
            }, new ComplexPoco
            {
                MyStrings = new[] { "1", "1" }
            });

            // Assert
            Assert.AreEqual(false, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_False_ComplexPoco_DictionaryNotEqual_NullDictionary1()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            // Act
            var actual = componentUnderTest.Equals(new ComplexPoco
            {
                SimplePocoDictionary = null
            }, new ComplexPoco
            {
                SimplePocoDictionary = new Dictionary<string, SimplePoco>
                {
                    {"2", new SimplePoco() }
                }
            });

            // Assert
            Assert.AreEqual(false, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_False_ComplexPoco_DictionaryNotEqual_NullDictionary2()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            // Act
            var actual = componentUnderTest.Equals(new ComplexPoco
            {
                SimplePocoDictionary = new Dictionary<string, SimplePoco>
                {
                    {"2", new SimplePoco() }
                }
            }, new ComplexPoco
            {
                SimplePocoDictionary = null
            });

            // Assert
            Assert.AreEqual(false, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_False_ComplexPoco_DictionaryNotEqual_KeyDifferent()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            // Act
            var actual = componentUnderTest.Equals(new ComplexPoco
            {
                SimplePocoDictionary = new Dictionary<string, SimplePoco>
                {
                    {"1", new SimplePoco() }
                }
            }, new ComplexPoco
            {
                SimplePocoDictionary = new Dictionary<string, SimplePoco>
                {
                    {"2", new SimplePoco() }
                }
            });

            // Assert
            Assert.AreEqual(false, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_False_ComplexPoco_DictionaryNotEqual_ValueDifferent()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            // Act
            var actual = componentUnderTest.Equals(new ComplexPoco
            {
                SimplePocoDictionary = new Dictionary<string, SimplePoco>
                {
                    {"1", new SimplePoco() }
                }
            }, new ComplexPoco
            {
                SimplePocoDictionary = new Dictionary<string, SimplePoco>
                {
                    {"1", new SimplePoco {MyString = "set"} }
                }
            });

            // Assert
            Assert.AreEqual(false, actual);
        }

        #endregion


        #region Lists


        [TestMethod, TestCategory("Unit")]
        public void Equals_True_List_SimpleType()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            var list1 = new[]
            {
                "a","b","c"
            };

            var list2 = new[]
            {
                "a","b","c"
            };

            // Act
            var actual = componentUnderTest.Equals(list1, list2);

            // Assert
            Assert.AreEqual(true, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_False_List_SimpleType()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            var list1 = new[]
            {
                "a","b","c"
            };

            var list2 = new[]
            {
                "a","a","a"
            };

            // Act
            var actual = componentUnderTest.Equals(list1, list2);

            // Assert
            Assert.AreEqual(false, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_True_List_SingleLevel()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            var list1 = new[]
            {
                new SimplePoco
                {
                    MyInt = 1,
                    MyString = "test",
                    MyIgnoredString = "xxx",
                    MyDateTime = new DateTime(2000, 1, 1)
                },
                new SimplePoco
                {
                    MyInt = 1,
                    MyString = "test",
                    MyIgnoredString = "xxx",
                    MyDateTime = new DateTime(2000, 1, 1)
                }
            };

            var list2 = new[]
            {
                new SimplePoco
                {
                    MyInt = 1,
                    MyString = "test",
                    MyIgnoredString = "xxx",
                    MyDateTime = new DateTime(2000, 1, 1)
                },
                new SimplePoco
                {
                    MyInt = 1,
                    MyString = "test",
                    MyIgnoredString = "xxx",
                    MyDateTime = new DateTime(2000, 1, 1)
                }
            };

            // Act
            var actual = componentUnderTest.Equals(list1, list2);

            // Assert
            Assert.AreEqual(true, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_False_List_SingleLevel()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            var list1 = new[]
            {
                new SimplePoco
                {
                    MyInt = 1,
                    MyString = "test",
                    MyIgnoredString = "xxx",
                    MyDateTime = new DateTime(2000, 1, 1)
                },
                new SimplePoco
                {
                    MyInt = 1,
                    MyString = "test",
                    MyIgnoredString = "xxx",
                    MyDateTime = new DateTime(2000, 1, 1)
                }
            };

            var list2 = new[]
            {
                new SimplePoco
                {
                    MyInt = 1,
                    MyString = "test other",
                    MyIgnoredString = "xxx",
                    MyDateTime = new DateTime(2000, 1, 1)
                },
                new SimplePoco
                {
                    MyInt = 1,
                    MyString = "test",
                    MyIgnoredString = "xxx",
                    MyDateTime = new DateTime(2000, 1, 1)
                }
            };

            // Act
            var actual = componentUnderTest.Equals(list1, list2);

            // Assert
            Assert.AreEqual(false, actual);
        }

        #endregion


        #region CircularReference

        [TestMethod, TestCategory("Unit")]
        public void Equals_True_CircularReference()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            var parent1 = new CircularReference();
            var child1 = new CircularReference();
            parent1.Child = child1;
            child1.Parent = parent1;

            var parent2 = new CircularReference();
            var child2 = new CircularReference();
            parent2.Child = child2;
            child2.Parent = parent2;

            // Act
            var actual = componentUnderTest.Equals(parent1, parent2);

            // Assert
            Assert.AreEqual(true, actual);
        }

        [TestMethod, TestCategory("Unit")]
        public void Equals_False_CircularReference()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            var parent1 = new CircularReference();
            var child1 = new CircularReference();
            parent1.Child = child1;
            child1.Parent = parent1;
            child1.MyString = "test";

            var parent2 = new CircularReference();
            var child2 = new CircularReference();
            parent2.Child = child2;
            child2.Parent = parent2;

            // Act
            var actual = componentUnderTest.Equals(parent1, parent2);

            // Assert
            Assert.AreEqual(false, actual);
        }

        [TestMethod, TestCategory("Unit")]
        public void Equals_True_CircularReference_DoubleNavigation()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            var parent1 = new CircularReference();
            var child1 = new CircularReference();
            var other1 = new CircularReference();
            parent1.Child = child1;
            child1.Parent = parent1;
            child1.OtheReference = other1;
            other1.Child = parent1;

            var parent2 = new CircularReference();
            var child2 = new CircularReference();
            var other2 = new CircularReference();
            parent2.Child = child2;
            child2.Parent = parent2;
            child2.OtheReference = other2;
            other2.Child = parent2;

            // Act
            var actual = componentUnderTest.Equals(parent1, parent2);

            // Assert
            Assert.AreEqual(true, actual);
        }

        #endregion
    }
}