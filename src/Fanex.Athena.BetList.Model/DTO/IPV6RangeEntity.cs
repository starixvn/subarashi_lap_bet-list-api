namespace Fanex.Athena.BetList.Model.DTO
{
    using System.Globalization;
    using System.Numerics;

    public record IPV6RangeEntity
    {
        public string IPFrom { get; set; }

        public string IPTo { get; set; }

        public BigInteger FromNumber
            => BigInteger.Parse(IPFrom, NumberStyles.AllowHexSpecifier);

        public BigInteger ToNumber
            => BigInteger.Parse(IPTo, NumberStyles.AllowHexSpecifier);
    }
}