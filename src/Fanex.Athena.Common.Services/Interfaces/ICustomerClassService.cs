namespace Fanex.Athena.Common.Services.Interfaces
{
    public interface ICustomerClassService
    {
        Task<IEnumerable<string>> GetAllCustomerClassId();
    }
}
