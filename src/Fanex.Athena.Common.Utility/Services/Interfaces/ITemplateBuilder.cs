namespace Fanex.Athena.Common.Utility.Services.Interfaces
{
    public interface ITemplateBuilder
    {
        string BuildContent<T>(string path, T model);
    }
}