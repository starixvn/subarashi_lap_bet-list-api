using Fanex.Athena.TestHelper.Constraints;

namespace Fanex.Athena.TestHelper.Extensions
{
    public class Tobe : Is
    {
        public static SamePropertiesConstraint SameProperties(object expected)
            => new SamePropertiesConstraint(expected);

    }
}
