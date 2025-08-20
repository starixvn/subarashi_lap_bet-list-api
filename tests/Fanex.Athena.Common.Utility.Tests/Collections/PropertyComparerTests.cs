namespace Fanex.Athena.Common.Utility.Tests.Collections
{
    using System.ComponentModel;
    using ClassTests;
    using Utility.Collections;

    public class PropertyComparerTests
    {
        [TestCase("Id", -1)]
        [TestCase("NickName", 0)]
        [TestCase("Name", 1)]
        public void Compare_AscendingDirection_ReturnCorrectValue(string propertyName, int expected)
        {
            // Arrange
            var comparer = new PropertyComparer<Person>(propertyName, ListSortDirection.Ascending);
            var firstObject = new Person { Id = 1, Name = "Walter", NickName = "xxx" };
            var secondObject = new Person { Id = 2, Name = "Ralph", NickName = "xxx" };

            // Action
            var result = comparer.Compare(firstObject, secondObject);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("Id", 1)]
        [TestCase("NickName", 0)]
        [TestCase("Name", -1)]
        public void Compare_DescendingDirection_ReturnCorrectValue(string propertyName, int expected)
        {
            // Arrange
            var comparer = new PropertyComparer<Person>(propertyName, ListSortDirection.Descending);
            var firstObject = new Person { Id = 1, Name = "Walter", NickName = "xxx" };
            var secondObject = new Person { Id = 2, Name = "Ralph", NickName = "xxx" };

            // Action
            var result = comparer.Compare(firstObject, secondObject);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Compare_PropertyNotExistInFirstObject_ReturnCorrectValue()
        {
            // Arrange
            var comparer = new PropertyComparer<Person>("NickName", ListSortDirection.Ascending);
            var firstObject = new Person { Id = 1, Name = "Walter" };
            var secondObject = new Person { Id = 2, Name = "Ralph", NickName = "xxx" };
            var expected = -1;

            // Action
            var result = comparer.Compare(firstObject, secondObject);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Compare_PropertyNotExistInSecondObject_ReturnCorrectValue()
        {
            // Arrange
            var comparer = new PropertyComparer<Person>("NickName", ListSortDirection.Ascending);
            var firstObject = new Person { Id = 1, Name = "Walter", NickName = "xxx" };
            var secondObject = new Person { Id = 2, Name = "Ralph" };
            var expected = 0;

            // Action
            var result = comparer.Compare(firstObject, secondObject);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
