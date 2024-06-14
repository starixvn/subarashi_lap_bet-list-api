

namespace Fanex.Athena.Common.Model.DTO
{
    public class RiskLevel
    {
        public string? Id { get; set; }

        public String? Key { get; set; }

        public String? Name { get; set; }

        public IEnumerable<string> CustomerClassList { get; set; } = new List<string>();
    }
}