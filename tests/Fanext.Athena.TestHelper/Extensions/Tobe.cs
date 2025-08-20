namespace Fanex.Athena.TestHelper.Extensions
{
    using Constraints;

    public class Tobe : Is
    {
        public static SamePropertiesConstraint SameProperties(object expected)
            => new SamePropertiesConstraint(expected);

    }
}
