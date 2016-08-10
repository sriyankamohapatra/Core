using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
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

        public class InheritedSimplePoco : SimplePoco
        {
            public string NyNewValue { get; set; }
        }

        public class ComplexPoco
        {
            public int MyInt { get; set; }

            public string[] MyStrings { get; set; }

            public SimplePoco SimplePoco { get; set; }

            public SimplePoco[] SimplePocoArray { get; set; }

            public DateTime? MyNullableDateTime { get; set; }

            public byte[] Bytes { get; set; }

            public Dictionary<string, SimplePoco> SimplePocoDictionary = new Dictionary<string, SimplePoco>();
        }


        public class CircularReference
        {
            public CircularReference Child { get; set; }

            public CircularReference Parent { get; set; }

            public string MyString { get; set; }

            public CircularReference OtheReference { get; set; }
        }

        public class InheritedSimplePocoTypeComparer : IFieldValueTypeEqualityComparer
        {
            public bool TreatAsSameType(object lhs, object rhs)
            {
                return lhs is SimplePoco && rhs is SimplePoco;
            }
        }

        public class SimplePocoEqualityComparer : IFieldValueEqualityComparer
        {
            public new bool Equals(object x, object y)
            {
                return true;
            }

            public int GetHashCode(object obj)
            {
                throw new NotImplementedException();
            }

            public bool CanCompare(object lhs, object rhs)
            {
                return lhs is SimplePoco && rhs is SimplePoco;
            }

            public bool CanCompare(ref object lhsField, ref object rhsField, object lhsParent, object rhsParent, FieldInfo field)
            {
                return lhsField is SimplePoco && rhsField is SimplePoco;
            }
        }

        public class GenericType<T>
        {
            public T Instance { get; set; }
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


        [TestMethod, TestCategory("Unit")]
        public void Equals_True_Bytes()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();

            // Act
            var actual = componentUnderTest.Equals(Encoding.Default.GetBytes("test"), Encoding.Default.GetBytes("test"));

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
                MyDateTime = new DateTime(2000, 1, 1).AddMilliseconds(999)
            }, new SimplePoco
            {
                MyInt = 1,
                MyString = "test",
                MyIgnoredString = "xxx",
                MyDateTime = new DateTime(2000, 1, 1).AddMilliseconds(999)
            });

            // Assert
            Assert.AreEqual(true, actual);
        }

        [TestMethod, TestCategory("Unit")]
        public void Equals_True_SimplePoco_ValuesSet_SqlServerDateTimeEqualityComparer()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });
            FieldValueEqualityComparer.UseDateTimeEqualityComparer(new SqlServerDateTimeEqualityComparer());

            // Act
            var actual = componentUnderTest.Equals(new SimplePoco
            {
                MyInt = 1,
                MyString = "test",
                MyIgnoredString = "xxx",
                MyDateTime = new DateTime(2000, 1, 1, 0, 0, 1).AddMilliseconds(999)
            }, new SimplePoco
            {
                MyInt = 1,
                MyString = "test",
                MyIgnoredString = "xxx",
                MyDateTime = new DateTime(2000, 1, 1, 0, 0, 2).AddMilliseconds(998)
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

        [TestMethod, TestCategory("Unit")]
        public void Equals_True_SimplePoco_ValuesSet_Lists()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            var list1 = new List<SimplePoco>
            {
                new SimplePoco
                {
                    MyInt = 1,
                    MyString = "test",
                    MyIgnoredString = "some value",
                    MyDateTime = new DateTime(2000, 1, 1)
                }
            };
            var list2 = new HashSet<SimplePoco>
            {
                new SimplePoco
                {
                    MyInt = 1,
                    MyString = "test",
                    MyIgnoredString = "some value",
                    MyDateTime = new DateTime(2000, 1, 1)
                }
            };

            // Act
            var actual = componentUnderTest.Equals(list1, list2);

            // Assert
            Assert.AreEqual(true, actual);
        }

        [TestMethod, TestCategory("Unit")]
        public void Equals_False_SimplePoco_ValuesSet_Lists()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            var list1 = new List<SimplePoco>
            {
                new SimplePoco
                {
                    MyInt = 1,
                    MyString = "test",
                    MyIgnoredString = "some value",
                    MyDateTime = new DateTime(2000, 1, 1)
                }
            };
            var list2 = new HashSet<SimplePoco>
            {
                new SimplePoco
                {
                    MyInt = 2,
                    MyString = "test",
                    MyIgnoredString = "some value",
                    MyDateTime = new DateTime(2000, 1, 1)
                }
            };

            // Act
            var actual = componentUnderTest.Equals(list1, list2);

            // Assert
            Assert.AreEqual(false, actual);
        }

        [TestMethod, TestCategory("Unit")]
        public void Equals_True_SimplePoco_ValuesSet_Dictionaries()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            var list1 = new Dictionary<string, SimplePoco>
            {
                {"1", new SimplePoco
                {
                    MyInt = 1,
                    MyString = "test",
                    MyIgnoredString = "some value",
                    MyDateTime = new DateTime(2000, 1, 1)
                }
            }};
            var list2 = new Dictionary<string, SimplePoco>
            {
                {"1", new SimplePoco
                {
                    MyInt = 1,
                    MyString = "test",
                    MyIgnoredString = "some value",
                    MyDateTime = new DateTime(2000, 1, 1)
                }
            }};

            // Act
            var actual = componentUnderTest.Equals(list1, list2);

            // Assert
            Assert.AreEqual(true, actual);
        }

        [TestMethod, TestCategory("Unit")]
        public void Equals_False_SimplePoco_ValuesSet_Dictionaries()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            var list1 = new Dictionary<string, SimplePoco>
            {
                {"1", new SimplePoco
                {
                    MyInt = 1,
                    MyString = "test",
                    MyIgnoredString = "some value",
                    MyDateTime = new DateTime(2000, 1, 1)
                }
            }};
            var list2 = new Dictionary<string, SimplePoco>
            {
                {"1", new SimplePoco
                {
                    MyInt = 2,
                    MyString = "test",
                    MyIgnoredString = "some value",
                    MyDateTime = new DateTime(2000, 1, 1)
                }
            }};

            // Act
            var actual = componentUnderTest.Equals(list1, list2);

            // Assert
            Assert.AreEqual(false, actual);
        }

        #endregion


        #region ComplexPoco


        [TestMethod, TestCategory("Unit")]
        public void Equals_True_ComplexPoco()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });
            var simplePoco = new SimplePoco { MyInt = 23 };

            // Act
            var actual = componentUnderTest.Equals(new ComplexPoco
            {
                MyInt = 1,
                MyStrings = new[] { "1", "2" },
                SimplePocoArray = new[] { new SimplePoco(), simplePoco },
                SimplePocoDictionary = new Dictionary<string, SimplePoco>
                {
                    {"1", new SimplePoco {MyString = "test"}},
                    {"2", simplePoco}
                },
                Bytes = Encoding.Default.GetBytes("test")
            }, new ComplexPoco
            {
                MyInt = 1,
                MyStrings = new[] { "1", "2" },
                SimplePocoArray = new[] { new SimplePoco(), simplePoco },
                SimplePocoDictionary = new Dictionary<string, SimplePoco>
                {
                    {"1", new SimplePoco {MyString = "test"}},
                    {"2", simplePoco}
                },
                Bytes = Encoding.Default.GetBytes("test")
            });

            // Assert
            Assert.AreEqual(true, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void AreEqual_True_ComplexPoco()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });
            var simplePoco = new SimplePoco { MyInt = 23 };
            var lhs = new ComplexPoco
            {
                MyInt = 1,
                MyStrings = new[] { "1", "2" },
                SimplePocoArray = new[] { new SimplePoco(), simplePoco },
                SimplePocoDictionary = new Dictionary<string, SimplePoco>
                {
                    {"1", new SimplePoco {MyString = "test"}},
                    {"2", simplePoco}
                },
                Bytes = Encoding.Default.GetBytes("test")
            };
            var rhs = new ComplexPoco
            {
                MyInt = 1,
                MyStrings = new[] { "1", "2" },
                SimplePocoArray = new[] { new SimplePoco(), simplePoco },
                SimplePocoDictionary = new Dictionary<string, SimplePoco>
                {
                    {"1", new SimplePoco {MyString = "test"}},
                    {"2", simplePoco}
                },
                Bytes = Encoding.Default.GetBytes("test")
            };

            // Act
            var actual = FieldValueEqualityComparer.AreEqual(lhs, rhs);

            // Assert
            Assert.AreEqual(true, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_True_ComplexPoco_ArraysEqual_BothNull()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            // Act
            var actual = componentUnderTest.Equals(new ComplexPoco
            {
                MyStrings = null
            }, new ComplexPoco
            {
                MyStrings = null
            });

            // Assert
            Assert.AreEqual(true, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_True_ComplexPoco_ArraysEqual_BothSameReference()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            var strings = new[] { "1", "2" };

            // Act
            var actual = componentUnderTest.Equals(new ComplexPoco
            {
                MyStrings = strings
            }, new ComplexPoco
            {
                MyStrings = strings
            });

            // Assert
            Assert.AreEqual(true, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_False_ComplexPoco_ArraysNotEqual_Null_Lhs()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            // Act
            var actual = componentUnderTest.Equals(new ComplexPoco
            {
                MyStrings = null
            }, new ComplexPoco
            {
                MyStrings = new string[0]
            });

            // Assert
            Assert.AreEqual(false, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_False_ComplexPoco_ArraysNotEqual_Null_Rhs()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            // Act
            var actual = componentUnderTest.Equals(new ComplexPoco
            {
                MyStrings = new string[0]
            }, new ComplexPoco
            {
                MyStrings = null
            });

            // Assert
            Assert.AreEqual(false, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_False_ComplexPoco_ArraysNotEqual_DifferentValues()
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
        public void Equals_False_ComplexPoco_ArraysNotEqual_DifferentValues_Null_Lhs()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            // Act
            var actual = componentUnderTest.Equals(new ComplexPoco
            {
                MyStrings = new[] { "1", null }
            }, new ComplexPoco
            {
                MyStrings = new[] { "1", "2" }
            });

            // Assert
            Assert.AreEqual(false, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_False_ComplexPoco_ArraysNotEqual_DifferentValues_Null_Rhs()
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
                MyStrings = new[] { "1", null }
            });

            // Assert
            Assert.AreEqual(false, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_False_ComplexPoco_ArraysNotEqual_DifferentSizes()
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
                MyStrings = new[] { "1" }
            });

            // Assert
            Assert.AreEqual(false, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_True_ComplexPoco_InDictionary()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            var lhs = new Dictionary<string, ComplexPoco>
            {
                {"a", new ComplexPoco()},
                {"b", new ComplexPoco()}
            };
            var rhs = new Dictionary<string, ComplexPoco>
            {
                {"a", new ComplexPoco()},
                {"b", new ComplexPoco()}
            };

            // Act
            var actual = componentUnderTest.Equals(lhs, rhs);

            // Assert
            Assert.AreEqual(true, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_False_ComplexPoco_InDictionary_ValuesDifferent()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            var lhs = new Dictionary<string, ComplexPoco>
            {
                {"a", new ComplexPoco()},
                {"b", new ComplexPoco {MyInt = 1} }
            };
            var rhs = new Dictionary<string, ComplexPoco>
            {
                {"a", new ComplexPoco()},
                {"b", new ComplexPoco{MyInt = 2}}
            };

            // Act
            var actual = componentUnderTest.Equals(lhs, rhs);

            // Assert
            Assert.AreEqual(false, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_False_ComplexPoco_InDictionary_KeysDifferent()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            var lhs = new Dictionary<string, ComplexPoco>
            {
                {"a", new ComplexPoco()},
                {"b", new ComplexPoco()}
            };
            var rhs = new Dictionary<string, ComplexPoco>
            {
                {"a", new ComplexPoco()},
                {"c", new ComplexPoco()}
            };

            // Act
            var actual = componentUnderTest.Equals(lhs, rhs);

            // Assert
            Assert.AreEqual(false, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_False_ComplexPoco_InDictionary_RhsNull()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            var lhs = new Dictionary<string, ComplexPoco>
            {
                {"a", new ComplexPoco()},
                {"b", new ComplexPoco()}
            };

            // Act
            var actual = componentUnderTest.Equals(lhs, null);

            // Assert
            Assert.AreEqual(false, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_True_ComplexPoco_InList()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            var lhs = new List<ComplexPoco>
            {
                new ComplexPoco(),
                new ComplexPoco()
            };
            var rhs = new List <ComplexPoco>
            {
                new ComplexPoco(),
                new ComplexPoco()
            };

            // Act
            var actual = componentUnderTest.Equals(lhs, rhs);

            // Assert
            Assert.AreEqual(true, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_False_ComplexPoco_InList_ValuesDifferent()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            var lhs = new List<ComplexPoco>
            {
                new ComplexPoco(),
                new ComplexPoco { MyInt = 1}
            };
            var rhs = new List<ComplexPoco>
            {
                new ComplexPoco(),
                new ComplexPoco { MyInt = 2}
            };

            // Act
            var actual = componentUnderTest.Equals(lhs, rhs);

            // Assert
            Assert.AreEqual(false, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_False_ComplexPoco_DictionaryNotEqual_NullDictionary_Lhs()
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
        public void Equals_False_ComplexPoco_DictionaryNotEqual_NullDictionary_Rhs()
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
        public void Equals_True_ComplexPoco_DictionaryNotEqual()
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
        public void Equals_False_ComplexPoco_DictionaryNotEqual_ValueDifferent_Lhs()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            // Act
            var actual = componentUnderTest.Equals(new ComplexPoco
            {
                SimplePocoDictionary = new Dictionary<string, SimplePoco>
                {
                    {"1", new SimplePoco {MyString = "set"} }
                }
            }, new ComplexPoco
            {
                SimplePocoDictionary = new Dictionary<string, SimplePoco>
                {
                    {"1", new SimplePoco() }
                }
            });

            // Assert
            Assert.AreEqual(false, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_False_ComplexPoco_DictionaryNotEqual_ValueDifferent_Rhs()
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


        [TestMethod, TestCategory("Unit")]
        public void Equals_False_ComplexPoco_NUllableDateTime_Lhs()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            // Act
            var actual = componentUnderTest.Equals(new ComplexPoco
            {
            }, new ComplexPoco
            {
                MyNullableDateTime = new DateTime()
            });

            // Assert
            Assert.AreEqual(false, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_False_ComplexPoco_NUllableDateTime_Rhs()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            // Act
            var actual = componentUnderTest.Equals(new ComplexPoco
            {
                MyNullableDateTime = new DateTime()
            }, new ComplexPoco
            {
            });

            // Assert
            Assert.AreEqual(false, actual);
        }

        #endregion


        #region Generic Types

        [TestMethod, TestCategory("Unit")]
        public void Equals_False_GenericType_ComplexPoco()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            // Act
            var actual = componentUnderTest.Equals(
                new GenericType<ComplexPoco>
                {
                    Instance = new ComplexPoco
                    {
                        MyStrings = new[] { "1", "2" }
                    }
                },
                new GenericType<ComplexPoco>
                {
                    Instance = new ComplexPoco
                    {
                        MyStrings = new[] { "1", "2" }
                    }
                });

            // Assert
            Assert.AreEqual(true, actual);
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


        #region Type Comparer

        [TestMethod, TestCategory("Unit")]
        public void Equals_False_WithNoTypeComparer()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            var lhs = new SimplePoco();
            var rhs = new InheritedSimplePoco();

            // Act
            var actual = componentUnderTest.Equals(lhs, rhs);

            // Assert
            Assert.AreEqual(false, actual);
        }

        [TestMethod, TestCategory("Unit")]
        public void Equals_True_WithTypeComparer()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            var lhs = new SimplePoco();
            var rhs = new InheritedSimplePoco();

            // Act
            FieldValueEqualityComparer.AddFieldValueTypeEqualityComparer(new InheritedSimplePocoTypeComparer());
            FieldValueEqualityComparer.AddFieldValueTypeEqualityComparer(new InheritedSimplePocoTypeComparer());
            var actual = componentUnderTest.Equals(lhs, rhs);

            // Assert
            Assert.AreEqual(true, actual);
        }

        #endregion


        #region Equality Comparer


        [TestMethod, TestCategory("Unit")]
        public void Equals_True_WithEqualityComparer_AtTopLevel()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            var lhs = new SimplePoco
            {
                MyString = "1"
            };
            var rhs = new InheritedSimplePoco
            {
                MyString = "xxx"
            };

            // Act
            FieldValueEqualityComparer.AddFieldValueEqualityComparer(new SimplePocoEqualityComparer());
            var actual = componentUnderTest.Equals(lhs, rhs);

            // Assert
            Assert.AreEqual(true, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_True_WithEqualityComparer_InList()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            var lhs = new List<SimplePoco>
            {
                new SimplePoco
                {
                    MyString = "1"
                }
            };
            var rhs = new List<SimplePoco>
            {
                new SimplePoco
                {
                    MyString = "xxx"
                }
            };

            // Act
            FieldValueEqualityComparer.AddFieldValueEqualityComparer(new SimplePocoEqualityComparer());
            var actual = componentUnderTest.Equals(lhs, rhs);

            // Assert
            Assert.AreEqual(true, actual);
        }


        [TestMethod, TestCategory("Unit")]
        public void Equals_True_WithEqualityComparer_InComplexType()
        {
            // Arrange
            var componentUnderTest = new FieldValueEqualityComparer();
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new[] { typeof(SimplePoco).Assembly });

            var lhs = new ComplexPoco
            {
                SimplePoco = new SimplePoco
                {
                    MyString = "1"
                }
            };
            var rhs = new ComplexPoco
            {
                SimplePoco = new SimplePoco
                {
                    MyString = "xxx"
                }
            };

            // Act
            FieldValueEqualityComparer.AddFieldValueEqualityComparer(new SimplePocoEqualityComparer());
            var actual = componentUnderTest.Equals(lhs, rhs);

            // Assert
            Assert.AreEqual(true, actual);
        }

        #endregion
    }
}