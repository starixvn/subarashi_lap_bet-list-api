namespace Fanex.Athena.Common.Utility.Services.Interfaces
{
    public interface ITemplateBuilder
    {
        Task<string> BuildContent<T>(string path, T model);
    }
}