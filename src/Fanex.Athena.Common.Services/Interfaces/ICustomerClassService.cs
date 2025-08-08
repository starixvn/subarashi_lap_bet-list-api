namespace Fanex.Athena.Common.Services.Interfaces
{
    using Model.DTO;

    public interface ICustomerClassService
    {
        Task<IEnumerable<string>> GetAllCustomerClassId();

        Task<IEnumerable<RiskLevel>> GetAllRiskLevel();
    }
}
