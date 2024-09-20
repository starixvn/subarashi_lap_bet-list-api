using Fanex.Athena.Common.DataAccess;
using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Model.MetaModel;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Utility.Extensions;
using Fanex.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fanex.Athena.Common.Site.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class AutoBindMetaModelAttribute : TypeFilterAttribute
    {
        public AutoBindMetaModelAttribute(
            bool bindAgent = false,
            bool bindPlayerCom = false,
            bool bindProductCodeList = false,
            bool applySubsidiaryBusiness = false,
            bool bindShowingBetFromIP = false,
            bool bindSpecialPlatform = false,
            bool bindBanlanceUpAndMoveBet = false)
            : base(typeof(AutoBindMetaModelImplAttribute))
        {
            Arguments = new object[] { bindAgent, bindPlayerCom, bindProductCodeList, applySubsidiaryBusiness, bindShowingBetFromIP, bindSpecialPlatform, bindBanlanceUpAndMoveBet };
        }
    }

    public class AutoBindMetaModelImplAttribute : ActionFilterAttribute
    {
        private readonly IMetaModelBindingService _bindingService;
        private readonly IContextService _contextService;
        private readonly ISubsidiaryService _subsidiaryService;
        private readonly IBalanceUpDataAccess _balanceUpDataAccess;

        private bool IsBindAgent { get; init; }
        private bool IsBindPlayerCom { get; init; }
        private bool IsBindProductCodeList { get; init; }
        private bool IsApplySubsidiaryBusiness { get; init; }
        private bool IsBindShowingBetFromIP { get; init; }
        private bool IsBindSpecialPlatform { get; init; }
        private bool IsBindBanlanceUpAndMoveBet { get; init; }

        private const string ProductSB = "SB";

        public AutoBindMetaModelImplAttribute(
            IMetaModelBindingService bindingService,
            IContextService contextService,
            ISubsidiaryService subsidiaryService,
            IBalanceUpDataAccess banlanceUpDataAccess,
            bool bindAgent,
            bool bindPlayerCom,
            bool bindProductCodeList,
            bool applySubsidiaryBusiness,
            bool bindShowingBetFromIP,
            bool isBindSpecialPlatform,
            bool isBindBanlanceUpAndMoveBet)
        {
            _bindingService = bindingService;
            _contextService = contextService;
            _subsidiaryService = subsidiaryService;
            _balanceUpDataAccess = banlanceUpDataAccess;

            IsBindAgent = bindAgent;
            IsBindPlayerCom = bindPlayerCom;
            IsBindProductCodeList = bindProductCodeList;
            IsApplySubsidiaryBusiness = applySubsidiaryBusiness;
            IsBindShowingBetFromIP = bindShowingBetFromIP;
            IsBindSpecialPlatform = isBindSpecialPlatform;
            IsBindBanlanceUpAndMoveBet = isBindBanlanceUpAndMoveBet;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (IsBindAgent)
            {
                await BindAgent(context);
            }

            if (IsBindPlayerCom && MetaModelHelper.IsMetaModel(context.ActionArguments, out IPlayerComMetaModel playerComMetaModel))
            {
                playerComMetaModel.HasPlayerCom = _contextService.IsAllowedCustomerComm;
            }

            if (IsBindShowingBetFromIP)
            {
                BindShowingBetFromIP(context);
            }

            if (IsBindProductCodeList)
            {
                BindProductCodeList(context.ActionArguments, _contextService.OfferedSubProductList);
            }

            if(IsBindSpecialPlatform)
            {
                BindSpecialPlatform(context);
            }

            if(IsBindBanlanceUpAndMoveBet)
            {
                BindBalanceUpAndMoveBet(context);
            }    

            await next();
        }


        private void BindShowingBetFromIP(ActionExecutingContext context)
        {
            if (MetaModelHelper.IsMetaModel(context.ActionArguments, out IMetaModelShowBetFromIP metaModelShowBetFromIP))
            {
                metaModelShowBetFromIP.IsShowIpvn = _contextService.IsShowIpvn;
                metaModelShowBetFromIP.IsShowIp = _contextService.IsShowIP;
            }
        }

        private async Task BindAgent(ActionExecutingContext context)
        {
            var agentList = _contextService.AgentList as IDictionary<int, int[]>;
            if (IsApplySubsidiaryBusiness && MetaModelHelper.IsMetaModel(context.ActionArguments, out ISubsidiarySearchMetaModel subsidiaryMetaModel)
                && !_subsidiaryService.IsIgnoredSubsidiarySearch(subsidiaryMetaModel.SubsidiarySearch, subsidiaryMetaModel.SubsidiaryList))
            {
                agentList = await _subsidiaryService.GetAgentList(subsidiaryMetaModel.SubsidiaryList);
            }
            _bindingService.BindAgentXml(context.ActionArguments, agentList);
        }

        private void BindSpecialPlatform(ActionExecutingContext context)
        {
            if (MetaModelHelper.IsMetaModel(context.ActionArguments, out ISpecialPlatformMetaModel specialPlatformMetaModel))
            {
                specialPlatformMetaModel.SpecialPlatform = _contextService.SpecialPlatform;
            }
        }


        public static void BindProductCodeList(IDictionary<string, object> actionArguments, IEnumerable<string> offeredSubProducts)
        {
            if (MetaModelHelper.IsMetaModel(actionArguments, out IMetaModelProductCodeList metaModelProductCodeList)
                && !string.IsNullOrEmpty(metaModelProductCodeList.ProductCodeList))
            {
                BindProductCodeList(metaModelProductCodeList, offeredSubProducts);
            }
        }

        public static void BindProductCodeList(IMetaModelProductCodeList metaModel, IEnumerable<string> offeredSubProducts)
        {
            metaModel.OriginalProductCodeList = metaModel.ProductCodeList;
            metaModel.ProductCodeList = GetAllowedProductCodeList(offeredSubProducts, metaModel.ProductCodeList);
        }

        private static string GetAllowedProductCodeList(IEnumerable<string> offeredSubProductList, string productCodes)
        {
            if (!offeredSubProductList.IsPresent() || !(productCodes?.Split(',')).IsPresent(_ => _ == ProductSB))
            {
                return productCodes;
            }

            return productCodes + "," + string.Join(",", offeredSubProductList);
        }

        private void BindBalanceUpAndMoveBet(ActionExecutingContext context)
        {
            if (MetaModelHelper.IsMetaModel(context.ActionArguments, out IBalanceUpMetaModel value))
            {
                var temp = _balanceUpDataAccess.GetBalanceUp();
                value.BalanceUpDay = temp.BalanceUpDay;
                value.MoveBetsDay = temp.MoveBetsDay;
            }
        }
    }


}
