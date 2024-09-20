using Fanex.Athena.Common.Model.DTO;

namespace Fanex.Athena.Common.Services.Interfaces
{
    public interface ICustomerClassService
    {
        Task<IEnumerable<string>> GetAllCustomerClassId();

        Task<IEnumerable<RiskLevel>> GetAllRiskLevel();
    }
}
