using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sfa.Core.Context;
using Sfa.Core.Logging;
using Sfa.Core.Reflection;

namespace Sfa.Core.Equality
{
    /// <summary>
    /// Enables field value comparison for types within specified assemblies.
    /// </summary>
    public class FieldValueEqualityComparer : IEqualityComparer
    {
        #region Fields

        private static readonly string LoggerCategory = typeof(FieldValueEqualityComparer).FullName;
        private static IList<Type> _typesToUseValueSemanticsOn = new List<Type>();
        private static readonly HashSet<Type> _previouslyMatchedTypeThatShouldUseSemantics = new HashSet<Type>();
        private static readonly HashSet<Type> _previouslyMatchedTypeThatShouldNotUseSemantics = new HashSet<Type>();
        private static readonly List<IFieldValueEqualityComparer> EqualityComparers = new List<IFieldValueEqualityComparer>();
        private static readonly List<IFieldValueTypeEqualityComparer> TypeEqualityComparers = new List<IFieldValueTypeEqualityComparer>();

        #endregion


        #region Setup

        /// <summary>
        /// Sets the assemblies with types to use value semantics on.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        public static void SetAssembliesWithTypesToUseValueSemanticsOn(IList<Assembly> assemblies)
        {
            _typesToUseValueSemanticsOn = (from assembly in assemblies
                                           from type in assembly.GetTypes()
                                           where !type.IsDefined<NoValueCompareAttribute>()
                                           select type).ToList();
            _previouslyMatchedTypeThatShouldUseSemantics.Clear();
            _previouslyMatchedTypeThatShouldNotUseSemantics.Clear();
        }

        /// <summary>
        /// Adds a new comparer.
        /// </summary>
        /// <param name="comparer">The comparer to add.</param>
        public static void AddFieldValueEqualityComparer(IFieldValueEqualityComparer comparer)
        {
            if (EqualityComparers.All(o => o.GetType() != comparer.GetType()))
            {
                EqualityComparers.Add(comparer);
            }
        }

        /// <summary>
        /// Adds a new comparer.
        /// </summary>
        /// <param name="comparer">The comparer to add.</param>
        public static void AddFieldValueTypeEqualityComparer(IFieldValueTypeEqualityComparer comparer)
        {
            if (TypeEqualityComparers.All(o => o.GetType() != comparer.GetType()))
            {
                TypeEqualityComparers.Add(comparer);
            }
        }

        #endregion


        #region Implementation of IEqualityComparer

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// <c>true</c> if the specified objects are equal; otherwise, <c>false</c>.
        /// </returns>
        public new bool Equals(object x, object y)
        {
            return Compare(x, y, new ObjectReferenceDictionary());
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// <c>true</c> if the specified objects are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool AreEqual(object x, object y)
        {
            return new FieldValueEqualityComparer().Equals(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <returns>
        /// A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
        public int GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Equality Internals

        private static bool Compare(object obj1, object obj2, ObjectReferenceDictionary recursiveObjects)
        {
            bool equals;

            if ((obj1 == null) || (obj2 == null))
            {
                equals = ((obj1 == null) && (obj2 == null));
                if (!equals)
                {
                    var type = (obj1 ?? obj2).GetType();
                    LogFailedComparison("at least one of the instances is null", type,
                        $"obj1 is{(obj1 == null ? "" : "n't")} null", $"obj2 is{(obj2 == null ? "" : "n't")} null");
                }
            }
            else
            {
                IFieldValueEqualityComparer comparer;
                if (MatchesComparer(obj1, obj2, out comparer) || AreObjectsSameType(obj1, obj2) || AreObjectsDictionaries(obj1, obj2) || AreObjectsEnumerables(obj1, obj2))
                {
                    var dictionary1 = obj1 as IDictionary;
                    if (comparer != null)
                    {
                        equals = comparer.Equals(obj1, obj2);
                    }
                    else if (obj1 is byte[])
                    {
                        var lhs = (byte[])obj1;
                        var rhs = (byte[])obj2;
                        equals = lhs.StructurallyEqual(rhs);
                    }
                    else if (dictionary1 != null)
                    {
                        var dictionary2 = (IDictionary)obj2;

                        equals = AlreadyCompared(dictionary1, dictionary2, recursiveObjects) ||
                            (PerformListFieldComparison(dictionary1.Keys, dictionary2.Keys, recursiveObjects) &&
                            (PerformListFieldComparison(dictionary1.Values, dictionary2.Values, recursiveObjects)));
                    }
                    else if (obj1 is IEnumerable && obj1.GetType() != typeof(string))
                    {
                        equals = AlreadyCompared(obj1, obj2, recursiveObjects) || PerformListFieldComparison((IEnumerable)obj1, obj2 as IEnumerable, recursiveObjects);
                    }
                    else if (ShouldFieldHaveValueSemanticEqualityPerformed(obj1.GetType()))
                    {
                        equals = AlreadyCompared(obj1, obj2, recursiveObjects) || PerformFieldByFieldComparision(obj1, obj2, recursiveObjects);
                    }
                    else
                    {
                        equals = obj1.Equals(obj2);
                    }
                }
                else
                {
                    LogFailedComparison("objects not viable for comparison", obj1.GetType(), obj1.GetType(), obj2.GetType());
                    equals = false;
                }
            }

            return (equals);
        }

        private static bool MatchesComparer(object lhs, object rhs, out IFieldValueEqualityComparer comparer)
        {
            comparer = EqualityComparers.FirstOrDefault(c => c.CanCompare(lhs, rhs));
            return comparer != null;
        }

        private static bool MatchesComparer(ref object lhsField, ref object rhsField, object lhsParent, object rhsParent, FieldInfo field, out IFieldValueEqualityComparer comparer)
        {
            comparer = null;
            foreach (var equalityComparer in EqualityComparers)
            {
                if (equalityComparer.CanCompare(ref lhsField, ref rhsField, lhsParent, rhsParent, field))
                {
                    comparer = equalityComparer;
                    break;
                }
            }

            return comparer != null;
        }

        private static bool AreObjectsSameType(object obj1, object obj2)
        {
            if (obj1.GetType() == obj2.GetType())
            {
                return true;
            }

            return TypeEqualityComparers.Any(comparer => comparer.TreatAsSameType(obj1, obj2));
        }

        private static bool AreObjectsDictionaries(object obj1, object obj2)
        {
            return (obj1 is IDictionary && obj2 is IDictionary);
        }

        private static bool AreObjectsEnumerables(object obj1, object obj2)
        {
            return (obj1 is IEnumerable && obj2 is IEnumerable);
        }

        private static bool AlreadyCompared(object target, object toCompare, ObjectReferenceDictionary recursiveObjects)
        {
            var alreadyCompared = false;

            if (recursiveObjects.ContainsKey(target))
            {
                if (ReferenceEquals(recursiveObjects[target], toCompare))
                {
                    alreadyCompared = true;
                }
            }
            else
            {
                recursiveObjects.Add(target, toCompare);
            }

            return alreadyCompared;
        }

        private static bool PerformFieldByFieldComparision(object obj1, object obj2, ObjectReferenceDictionary recursiveObjects)
        {
            var equals = true;

            foreach (var currentField in obj1.GetAllFieldsWithoutAttribute<NoValueCompareAttribute>())
            {
                var fieldValue1 = currentField.GetValue(obj1);
                var fieldValue2 = currentField.GetValue(obj2);

                IFieldValueEqualityComparer comparer;

                if (MatchesComparer(ref fieldValue1, ref fieldValue2, obj1, obj2, currentField, out comparer))
                {
                    equals = comparer.Equals(fieldValue1, fieldValue2);
                }
                else if (fieldValue1 as IDictionary != null) // Will not pass if null and can just use normal null checking
                {
                    var dictionary1 = (IDictionary) fieldValue1;
                    var dictionary2 = fieldValue2 as IDictionary;
                    if (dictionary2 == null)
                    {
                        equals = false;
                    }
                    else
                    {
                        equals = AlreadyCompared(dictionary1, dictionary2, recursiveObjects) ||
                            (PerformListFieldComparison(dictionary1.Keys, dictionary2.Keys, recursiveObjects) &&
                            (PerformListFieldComparison(dictionary1.Values, dictionary2.Values, recursiveObjects)));
                    }
                }
                else if (typeof(IEnumerable).IsAssignableFrom(currentField.FieldType) && typeof(string) != currentField.FieldType) // Will not pass if null and can just use normal null checking
                {
                    equals = PerformListFieldComparison(fieldValue1 as IEnumerable, fieldValue2 as IEnumerable, recursiveObjects);
                }
                else if ((currentField.FieldType) == typeof(DateTime) || (currentField.FieldType) == typeof(DateTime?))
                {
                    equals = PerformDateTimeFieldComparison(fieldValue1, fieldValue2);
                }
                else if (ShouldFieldHaveValueSemanticEqualityPerformed(currentField))
                {
                    equals = PerformEntityFieldComparison(fieldValue1, fieldValue2, recursiveObjects);
                }
                else
                {
                    equals = PerformBasicFieldComparison(fieldValue1, fieldValue2);
                }

                if (!equals)
                {
                    LogFailedComparison(currentField.Name, obj1.GetType(), fieldValue1, fieldValue2);

                    break;
                }
            }

            return equals;
        }

        private static bool PerformBasicFieldComparison(object value1, object value2)
        {
            bool equals;

            if ((value1 == null) || (value2 == null))
            {
                equals = ((value1 == null) && (value2 == null));
            }
            else
            {
                equals = value1.Equals(value2);
            }

            return equals;
        }

        private static bool PerformDateTimeFieldComparison(object value1, object value2)
        {
            bool equals;

            if ((value1 == null) || (value2 == null))
            {
                equals = ((value1 == null) && (value2 == null));
            }
            else
            {
                // Compare with no milliseconds.
                var dateTime1 = (DateTime)value1;
                var dateTime2 = (DateTime)value2;
                var dateTimeNoMilliSecs1 = new DateTime(dateTime1.Year, dateTime1.Month, dateTime1.Day, dateTime1.Hour, dateTime1.Minute, dateTime1.Second);
                var dateTimeNoMilliSecs2 = new DateTime(dateTime2.Year, dateTime2.Month, dateTime2.Day, dateTime2.Hour, dateTime2.Minute, dateTime2.Second);

                equals = dateTimeNoMilliSecs1.Equals(dateTimeNoMilliSecs2);
            }

            return equals;
        }

        private static bool PerformEntityFieldComparison(object value1, object value2, ObjectReferenceDictionary recursiveObjects)
        {
            bool equals;

            if ((value1 == null) || (value2 == null))
            {
                equals = ((value1 == null) && (value2 == null));
            }
            else
            {
                equals = Compare(value1, value2, recursiveObjects);
            }

            return equals;
        }

        private static bool PerformListFieldComparison(IEnumerable list1, IEnumerable list2, ObjectReferenceDictionary recursiveObjects, bool logOnFail = true)
        {
            bool equals;
            var failMessage = string.Empty;

            if ((list1 == null) || (list2 == null))
            {
                equals = ((list1 == null) && (list2 == null));
            }
            else if (ReferenceEquals(list1, list2))
            {
                equals = true;
            }
            else
            {
                equals = CompareListElements(list1, list2, recursiveObjects, ref failMessage);
            }

            if (!equals && logOnFail)
            {
                var type = list1?.GetType() ?? list2.GetType();

                LogFailedComparison(failMessage, type, list1, list2);
            }

            return (equals);
        }

        private static bool CompareListElements(IEnumerable enumerable1, IEnumerable enumerable2, ObjectReferenceDictionary recursiveObjects, ref string failMessage)
        {
            var areTheSame = true;

            var list1 = enumerable1 as IList<object> ?? enumerable1.Cast<object>().ToList();
            var list2 = enumerable2 as IList<object> ?? enumerable2.Cast<object>().ToList();

            if (list1.Count() != list2.Count())
            {
                areTheSame = false;
                failMessage = $"Expected length {list1.Count} but was {list2.Count}";
            }
            else
            {
                var enumerator1 = list1.GetEnumerator();
                var enumerator2 = list2.GetEnumerator();

                var index = 0;

                while (enumerator1.MoveNext() && enumerator2.MoveNext())
                {
                    var instance1 = enumerator1.Current;
                    var instance2 = enumerator2.Current;

                    IFieldValueEqualityComparer comparer;
                    if (instance1 == null)
                    {
                        areTheSame = (instance2 == null);
                    }
                    else if (MatchesComparer(instance1, instance2, out comparer))
                    {
                        areTheSame = comparer.Equals(instance1, instance2);
                    }
                    else if (ShouldFieldHaveValueSemanticEqualityPerformed(instance1.GetType()))
                    {
                        areTheSame = Compare(instance1, instance2, recursiveObjects);
                    }
                    else
                    {
                        areTheSame = instance1.Equals(instance2);
                    }

                    if (!areTheSame)
                    {
                        failMessage = $"Item's at index:{index} in the lists are not the same";
                        break;
                    }
                    index++;
                }
            }

            return (areTheSame);
        }

        private static void LogFailedComparison(string fieldName, Type fieldType, object value1, object value2)
        {
            ApplicationContext.Logger.Log(LoggingLevel.Debug, LoggerCategory, () =>
            {
                var value1AsString = value1?.ToString() ?? "null";
                var value2AsString = value2?.ToString() ?? "null";
                var message = $"Field '{fieldName}' of Type '{fieldType}' fails equality test.\nObject 1 has value: '{value1AsString}'\nObject 2 has value: '{value2AsString}'";
                return message;
            });
            ApplicationContext.Logger.Log(LoggingLevel.Debug, LoggerCategory, () => "**Have you remembered to add the list of assemblies that contain the types that should be checked for value semantics in the test setup?**");

        }

        private static bool ShouldFieldHaveValueSemanticEqualityPerformed(FieldInfo fieldInfo)
        {
            return ShouldFieldHaveValueSemanticEqualityPerformed(fieldInfo.FieldType);
        }

        private static bool ShouldFieldHaveValueSemanticEqualityPerformed(Type typeToFind)
        {
            var shouldFieldHaveValueSemanticEqualityPerformed = false;

            if (_previouslyMatchedTypeThatShouldUseSemantics.Contains(typeToFind))
            {
                shouldFieldHaveValueSemanticEqualityPerformed = true;
            }
            else if (!_previouslyMatchedTypeThatShouldNotUseSemantics.Contains(typeToFind))
            {
                shouldFieldHaveValueSemanticEqualityPerformed = (from type in _typesToUseValueSemanticsOn
                                                                 where type == typeToFind || (type.IsGenericType && typeToFind.IsGenericType && typeToFind.GetGenericTypeDefinition() == type.GetGenericTypeDefinition())
                                                                 select type).FirstOrDefault() != null;

                if (shouldFieldHaveValueSemanticEqualityPerformed)
                {
                    _previouslyMatchedTypeThatShouldUseSemantics.Add(typeToFind);
                }
                else
                {
                    _previouslyMatchedTypeThatShouldNotUseSemantics.Add(typeToFind);
                }
            }

            if (!IsSystemValueType(typeToFind) && !shouldFieldHaveValueSemanticEqualityPerformed)
            {
                ApplicationContext.Logger.Log(LoggingLevel.Debug, LoggerCategory, () => "The Type {0} is not being considered for field level equality and Equals will be used instead. To add the type, set the assemblies with SetAssembliesWithTypesToUseValueSemanticsOn to point to assemblies that contains types that will be used for value level semantics.", typeToFind);
            }

            return shouldFieldHaveValueSemanticEqualityPerformed;
        }

        private static bool IsSystemValueType(Type type)
        {
            return typeof(string) == type || (type.Namespace == typeof(string).Namespace && type.IsValueType);
        }

        #endregion


        #region Helper Classes

        private class ObjectReferenceDictionary : Dictionary<object, object>
        {
            internal ObjectReferenceDictionary()
                : base(new ReferenceEqualityComparer<object>())
            {

            }
        }

        #endregion
    }
}