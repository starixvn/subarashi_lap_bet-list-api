using Fanex.Athena.Result.Model.MetaModel;

namespace Fanex.Athena.Result.Controller.ResultService.Interfaces
{
    public interface IThirdPartySportResultService
    {
        public string GetResultUrl(ResultPopupMetaModel metaModel, string configKey);
    }
}