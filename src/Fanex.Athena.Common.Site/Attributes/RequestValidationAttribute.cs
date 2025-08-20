namespace Fanex.Athena.Common.Site.Attributes
{
    using System.Net;
    using Core.Utils;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Services.Interfaces;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RequestValidationAttribute : TypeFilterAttribute
    {
        public RequestValidationAttribute(
            bool validateCurrency = false,
            bool validateCurrencyList = false)
            : base(typeof(DataValidationImplAttribute))
        {
            Arguments = [validateCurrency, validateCurrencyList];
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class DataValidationImplAttribute : ActionFilterAttribute
    {
        private const string ErrorMsg = "Invalid Params";

        private readonly IContextService _contextService;
        private readonly ICurrencyValidation _currencyValidation;
        private readonly ILogger<DataValidationImplAttribute> _logger;

        private readonly bool _validateCurrency;
        private readonly bool _validateCurrencyList;

        public DataValidationImplAttribute(IContextService contextService, ICurrencyValidation currencyValidation, ILoggerFactory loggerFactory,
            bool validateCurrency,
            bool validateCurrencyList
            )
        {
            _logger = loggerFactory.CreateLogger<DataValidationImplAttribute>();
            _contextService = contextService;
            _currencyValidation = currencyValidation;

            _validateCurrency = validateCurrency;
            _validateCurrencyList = validateCurrencyList;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var isValid = IsValid(context.ActionArguments);
            if (!isValid)
            {
                _logger.LogWarning($"{ErrorMsg}: \n" +
                    $"Model: {JsonConvert.SerializeObject(context.ActionArguments)}\n" +
                    $"Context: {JsonConvert.SerializeObject(_contextService)}");
                context.Result = new JsonResult(new { Msg = ErrorMsg, IsSuccess = false })
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
            else
            {
                await next();
            }
        }

        private bool IsValid(IDictionary<string, object> actionArguments)
        {
            var result = !MetaModelHelper.IsMetaModel(actionArguments, out IValidationMetaModel validationMetaModel) || validationMetaModel.IsValid;

            result &= !_validateCurrency || MetaModelHelper.IsMetaModel(actionArguments, out ICurrencyIdMetaModel currencyIdMetaModel)
               && _currencyValidation.IsValidCurrency(currencyIdMetaModel.CurrencyId, _contextService.CurrencyList as IDictionary<int, string>);

            result &= !_validateCurrencyList || MetaModelHelper.IsMetaModel(actionArguments, out ICurrencyIdListMetaModel currencyIdListMetaModel)
                && _currencyValidation.IsValidCurrencyList(currencyIdListMetaModel.CurrencyIdList, _contextService.CurrencyList as IDictionary<int, string>);

            return result;
        }
    }
}
