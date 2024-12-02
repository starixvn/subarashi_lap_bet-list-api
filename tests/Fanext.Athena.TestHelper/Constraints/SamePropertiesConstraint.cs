using NUnit.Framework.Constraints;

namespace Fanex.Athena.TestHelper.Constraints
{
    public class SamePropertiesConstraint : Constraint
    {
        private readonly object expected;

        public SamePropertiesConstraint(object expected) : base(new[]
        {
            expected
        })
        {
            this.expected = expected;
        }

        public override string Description
        {
            get
            {
                return "same as " + expected;
            }
        }

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            bool isSuccess = ObjectDumper.Dump(expected) == ObjectDumper.Dump(actual);
            return new ConstraintResult(this, actual, isSuccess);
        }
    }
}