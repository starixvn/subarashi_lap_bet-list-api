namespace Fanex.Athena.Common.Utility.Tests.Extensions
{
    using Utility.Extensions;

    public class EnumerableExtensionTests
    {
        [Test]
        public void IsPresent_ShouldReturnFalse_WhenSourceIsNull()
        {
            IEnumerable<int> source = null;

            bool result = source.IsPresent();

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPresent_ShouldReturnFalse_WhenSourceIsEmpty()
        {
            IEnumerable<int> source = Enumerable.Empty<int>();

            bool result = source.IsPresent();

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPresent_ShouldReturnTrue_WhenSourceIsNotEmpty()
        {
            IEnumerable<int> source = new List<int> { 1, 2, 3 };

            bool result = source.IsPresent();

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsPresent_WithPredicate_ShouldReturnFalse_WhenSourceIsNull()
        {
            IEnumerable<int> source = null;

            bool result = source.IsPresent(x => x > 0);

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPresent_WithPredicate_ShouldReturnFalse_WhenSourceIsEmpty()
        {
            IEnumerable<int> source = Enumerable.Empty<int>();

            bool result = source.IsPresent(x => x > 0);

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPresent_WithPredicate_ShouldReturnFalse_WhenNoElementSatisfiesThePredicate()
        {
            IEnumerable<int> source = new List<int> { 1, 2, 3 };

            bool result = source.IsPresent(x => x > 10);

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPresent_WithPredicate_ShouldReturnTrue_WhenAtLeastOneElementSatisfiesThePredicate()
        {
            IEnumerable<int> source = new List<int> { 1, 2, 3 };

            bool result = source.IsPresent(x => x > 2);

            Assert.That(result, Is.True);
        }
    }
}
