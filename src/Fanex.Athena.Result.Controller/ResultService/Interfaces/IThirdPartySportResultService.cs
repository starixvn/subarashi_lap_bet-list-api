using Fanex.Athena.Result.Model.MetaModel;

namespace Fanex.Athena.Result.Controller.ResultService.Interfaces
{
    public interface IThirdPartySportResultService
    {
        public Uri GetResultUrl(ResultPopupMetaModel metaModel, string configKey);
    }
}