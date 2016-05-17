using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Sfa.Core.Testing
{
    [TestClass]
    public class BaseNSubstituteApplicationLogicTestTests : BaseNSubstituteApplicationLogicTest
    {
        protected override IEnumerable<Assembly> AssembliesWithTypesToPerformFieldValueEqualityOn
        {
            get
            {
                yield return typeof(Person).Assembly;
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void EnsureFieldValueEqualityWorks()
        {
            // Arrange
            var toReturn = new Person
            {
                First = "a",
                Last = "b"
            };
            var toReturn2 = new Person
            {
                First = "a",
                Last = "b"
            };
            var middle = new Person
            {
                First = "ax",
                Last = "b"
            };
            var expected = new Person
            {
                First = "a",
                Last = "b"
            };
            var mockPersonfactory = NewMock<IPersonFactory>();
            mockPersonfactory.CreatePerson("x", "y").Returns(toReturn);
            mockPersonfactory.UpdatePerson(middle).Returns(toReturn2);

            var sut = new MyTestClass(mockPersonfactory);

            // Act
            var actual = sut.MyTestMethod("x", "y");

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task EnsureFieldValueEqualityWorksWithAsync()
        {
            // Arrange
            var toReturn = new Person
            {
                First = "a",
                Last = "b"
            };
            var toReturn2 = new Person
            {
                First = "a",
                Last = "b"
            };
            var middle = new Person
            {
                First = "ax",
                Last = "b"
            };
            var expected = new Person
            {
                First = "a",
                Last = "b"
            };
            var mockPersonfactory = NewMock<IPersonFactory>();
            mockPersonfactory.CreatePersonAsync("x", "y").Returns(toReturn);
            mockPersonfactory.UpdatePersonAsync(middle).Returns(toReturn2);

            var sut = new MyTestClass(mockPersonfactory);

            // Act
            var actual = await sut.MyTestMethodAsync("x", "y");

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        public interface IPersonFactory
        {
            Person CreatePerson(string first, string last);
            Person UpdatePerson(Person personToUpdate);


            Task<Person> CreatePersonAsync(string first, string last);
            Task<Person> UpdatePersonAsync(Person personToUpdate);
        }

        public class Person
        {
            public string First { get; set; }
            public string Last { get; set; }
        }

        public class MyTestClass
        {
            private readonly IPersonFactory _personFactory;

            public MyTestClass(IPersonFactory personFactory)
            {
                _personFactory = personFactory;
            }

            public Person MyTestMethod(string first, string last)
            {
                var person = _personFactory.CreatePerson(first, last);
                person.First += "x";
                return _personFactory.UpdatePerson(person);
            }

            public async Task<Person> MyTestMethodAsync(string first, string last)
            {
                var person = await _personFactory.CreatePersonAsync(first, last);
                person.First += "x";
                return await _personFactory.UpdatePersonAsync(person);
            }
        }
    }
}