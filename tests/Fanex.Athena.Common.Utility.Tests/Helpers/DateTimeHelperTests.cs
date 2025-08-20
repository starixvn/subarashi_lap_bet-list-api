namespace Fanex.Athena.Common.Utility.Tests.Helpers
{
    using Utility.Helpers;

    public class DateTimeHelperTests
    {
        [Test]
        public void GetMaxDate_ShouldReturnGreaterDate()
        {
            // Arrange
            var dateOne = new DateTime(2022, 1, 1);
            var dateTwo = new DateTime(2022, 2, 1);

            // Act
            var result = DateTimeHelper.GetMaxDate(dateOne, dateTwo);

            // Assert
            Assert.That(result, Is.EqualTo(dateTwo));
        }

        [Test]
        public void GetMinDate_ShouldReturnSmallerDate()
        {
            // Arrange
            var dateOne = new DateTime(2022, 1, 1);
            var dateTwo = new DateTime(2022, 2, 1);

            // Act
            var result = DateTimeHelper.GetMinDate(dateOne, dateTwo);

            // Assert
            Assert.That(result, Is.EqualTo(dateOne));
        }

        [Test]
        public void IsBetween_ShouldReturnTrue_WhenDateIsBetweenMinAndMaxDate()
        {
            // Arrange
            var date = new DateTime(2022, 1, 15);
            var minDate = new DateTime(2022, 1, 1);
            var maxDate = new DateTime(2022, 1, 31);

            // Act
            var result = DateTimeHelper.IsBetween(date, minDate, maxDate);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsBetween_ShouldReturnFalse_WhenDateIsNotBetweenMinAndMaxDate()
        {
            // Arrange
            var date = new DateTime(2022, 2, 15);
            var minDate = new DateTime(2022, 1, 1);
            var maxDate = new DateTime(2022, 1, 31);

            // Act
            var result = DateTimeHelper.IsBetween(date, minDate, maxDate);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void GetFirstDateOfWeek_ShouldReturnFirstDateOfWeek()
        {
            // Arrange
            var date = new DateTime(2022, 1, 15);
            var startOfWeek = DayOfWeek.Monday;
            var expectedDate = new DateTime(2022, 1, 10);

            // Act
            var result = DateTimeHelper.GetFirstDateOfWeek(date, startOfWeek);

            // Assert
            Assert.That(result, Is.EqualTo(expectedDate));
        }

        [Test]
        public void IsValidMonthRange_ShouldReturnTrue_WhenToDateIsGreaterThanOrEqualToFromDateAndWithinMaximumMonthRange()
        {
            // Arrange
            var fromDate = new DateTime(2022, 1, 1);
            var toDate = new DateTime(2022, 3, 31);
            var maximumMonthRange = 3;

            // Act
            var result = DateTimeHelper.IsValidMonthRange(fromDate, toDate, maximumMonthRange);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsValidMonthRange_ShouldReturnFalse_WhenToDateIsLessThanFromDate()
        {
            // Arrange
            var fromDate = new DateTime(2022, 3, 31);
            var toDate = new DateTime(2022, 1, 1);
            var maximumMonthRange = 3;

            // Act
            var result = DateTimeHelper.IsValidMonthRange(fromDate, toDate, maximumMonthRange);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsValidMonthRange_ShouldReturnFalse_WhenMonthDifferenceIsGreaterThanMaximumMonthRange()
        {
            // Arrange
            var fromDate = new DateTime(2022, 1, 1);
            var toDate = new DateTime(2022, 5, 1);
            var maximumMonthRange = 3;

            // Act
            var result = DateTimeHelper.IsValidMonthRange(fromDate, toDate, maximumMonthRange);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ToBeginOfTheDay_ShouldReturnBeginningOfDay()
        {
            // Arrange
            var datetime = new DateTime(2022, 1, 15, 12, 30, 0);
            var expectedDatetime = new DateTime(2022, 1, 15, 0, 0, 0);

            // Act
            var result = DateTimeHelper.ToBeginOfTheDay(datetime);

            // Assert
            Assert.That(result, Is.EqualTo(expectedDatetime));
        }

        [Test]
        public void ToEndOfTheDay_ShouldReturnEndOfDay()
        {
            // Arrange
            var datetime = new DateTime(2022, 1, 15, 12, 30, 0);
            var expectedDatetime = new DateTime(2022, 1, 15, 23, 59, 59, 997);

            // Act
            var result = DateTimeHelper.ToEndOfTheDay(datetime);

            // Assert
            Assert.That(result, Is.EqualTo(expectedDatetime));
        }

        [Test]
        public void GetFirstDateOfMonth_ShouldReturnFirstDateOfMonth()
        {
            // Arrange
            var datetime = new DateTime(2022, 1, 15);
            var expectedDatetime = new DateTime(2022, 1, 1);

            // Act
            var result = DateTimeHelper.GetFirstDateOfMonth(datetime);

            // Assert
            Assert.That(result, Is.EqualTo(expectedDatetime));
        }

        [Test]
        public void DayOfPreviousWeek_ShouldReturnDayOfPreviousWeek()
        {
            // Arrange
            var datetime = new DateTime(2022, 1, 15);
            var dayOfWeek = DayOfWeek.Monday;
            var expectedDatetime = new DateTime(2022, 1, 10);

            // Act
            var result = datetime.DayOfPreviousWeek(dayOfWeek);

            // Assert
            Assert.That(result, Is.EqualTo(expectedDatetime));
        }

        [Test]
        public void DayOfNextWeek_ShouldReturnDayOfNextWeek()
        {
            // Arrange
            var datetime = new DateTime(2022, 1, 15);
            var dayOfWeek = DayOfWeek.Monday;
            var expectedDatetime = new DateTime(2022, 1, 17);

            // Act
            var result = datetime.DayOfNextWeek(dayOfWeek);

            // Assert
            Assert.That(result, Is.EqualTo(expectedDatetime));
        }

        [Test]
        public void Now_ShouldReturnCurrentDateTime()
        {
            // Arrange
            var expectedDateTime = DateTime.Now;
            DateTimeHelper.DateTimeOffsetProvider = () => expectedDateTime;

            // Act
            var result = DateTimeHelper.Now;

            // Assert
            Assert.That(result, Is.EqualTo(expectedDateTime));
        }
        [Test]
        public void Today_ShouldReturnCurrentDate()
        {
            // Arrange
            var expectedDate = DateTime.Now;
            DateTimeHelper.DateTimeOffsetProvider = () => expectedDate;

            // Act
            var result = DateTimeHelper.Today;

            // Assert
            Assert.That(result, Is.EqualTo(expectedDate.Date));
        }

        [Test]
        public void MinValue_ShouldReturnMinimumDateTimeValue()
        {
            // Arrange
            var expectedDateTime = DateTimeOffset.MinValue.DateTime;

            // Act
            var result = DateTimeHelper.MinValue;

            // Assert
            Assert.That(result, Is.EqualTo(expectedDateTime));
        }

        [Test]
        public void GetFirstDateOfWeek_ShouldReturnFirstDateOfWeek_WhenDateIsStartOfWeek()
        {
            // Arrange
            var date = new DateTime(2024, 11, 10);
            var startOfWeek = DayOfWeek.Monday;
            var expectedDate = new DateTime(2024, 11, 4);

            // Act
            var result = DateTimeHelper.GetFirstDateOfWeek(date, startOfWeek);

            // Assert
            Assert.That(result, Is.EqualTo(expectedDate));
        }
    }
}
