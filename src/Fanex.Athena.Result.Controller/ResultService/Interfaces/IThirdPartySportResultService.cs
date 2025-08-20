namespace Fanex.Athena.Result.Controller.ResultService.Interfaces
{
    using Model.MetaModel;

    public interface IThirdPartySportResultService
    {
        public Uri GetResultUrl(ResultPopupMetaModel metaModel, string configKey);
    }
}