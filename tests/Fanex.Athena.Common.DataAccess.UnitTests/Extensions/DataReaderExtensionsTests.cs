using Fanex.Athena.Common.DataAccess.Extensions;
using Fanex.Common.Extensions;
using NSubstitute;
using System.Data;

namespace Fanex.Athena.Common.DataAccess.UnitTests.Extensions
{
    [TestFixture]
    public class DataReaderExtensionsTests
    {
#pragma warning disable NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method
        private IDataReader _dataReader;
#pragma warning restore NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method

        [SetUp]
        public void SetUp()
        {
            _dataReader = Substitute.For<IDataReader>();
        }

        [TearDown]
        public void TearDown()
        {
            _dataReader = null;
        }

        [Test]
        public void HasColumn_ReturnsTrue_WhenColumnExists()
        {
            // Arrange
            _dataReader.FieldCount.Returns(1);
            _dataReader.GetName(0).Returns("TestColumn");

            // Act
            var result = _dataReader.HasColumn("TestColumn");

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void HasColumn_ReturnsFalse_WhenColumnDoesNotExist()
        {
            // Arrange
            _dataReader.FieldCount.Returns(1);
            _dataReader.GetName(0).Returns("TestColumn");

            // Act
            var result = _dataReader.HasColumn("NonExistentColumn");

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ToNullableInt_ReturnsIntValue_WhenFieldIsNotNull()
        {
            // Arrange
            _dataReader["TestField"].Returns(123);
            _dataReader.ToInt("TestField").Returns(123);

            // Act
            var result = _dataReader.ToNullableInt("TestField");

            // Assert
            Assert.That(result, Is.EqualTo(123));
        }

        [Test]
        public void ToNullableInt_ReturnsDefaultValue_WhenFieldIsNull()
        {
            // Arrange
            _dataReader["TestField"].Returns(DBNull.Value);

            // Act
            var result = _dataReader.ToNullableInt("TestField", 456);

            // Assert
            Assert.That(result, Is.EqualTo(456));
        }

        [Test]
        public void GetVal_ReturnsValue_WhenFieldIsNotNull()
        {
            // Arrange
            AddField("TestField");
            _dataReader.IsDBNull(Arg.Any<int>()).Returns(false);
            _dataReader["TestField"].Returns(123);

            // Act
            var result = _dataReader.GetVal("TestField", 456);

            // Assert
            Assert.That(result, Is.EqualTo(123));
        }

        [Test]
        public void GetVal_ReturnsDefaultValue_WhenFieldIsNull()
        {
            // Arrange
            AddField("TestField");
            _dataReader.IsDBNull(Arg.Any<int>()).Returns(true);

            // Act
            var result = _dataReader.GetVal("TestField", 456);

            // Assert
            Assert.That(result, Is.EqualTo(456));
        }

        [Test]
        public void GetVal_ReturnsDefaultValue_WhenColumnDoesNotExist()
        {
            // Act
            var result = _dataReader.GetVal("NonExistentField", 456);

            // Assert
            Assert.That(result, Is.EqualTo(456));
        }

        private void AddField(string fieldName)
        {
            if (_dataReader.FieldCount <= 0)
            {
                _dataReader.FieldCount.Returns(1);
            }
            else
            {
                _dataReader.FieldCount.Returns(_dataReader.FieldCount + 1);
            }
            var fieldCount = _dataReader.FieldCount;
            _dataReader.GetName(fieldCount - 1).Returns(fieldName);
        }
    }
}
