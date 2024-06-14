using System.Net;

namespace Fanex.Athena.Common.Utility.Services.Interfaces
{
    public interface IWebClientService
    {
        string UploadString(Uri address, string parameters);

        string DownloadString(string address);

        byte[] DownloadData(string address);

        string UploadString(Uri address, string parameters, IDictionary<HttpRequestHeader, string> headers);

        TOut UploadString<TIn, TOut>(string address, TIn requestData);
    }
}