using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.Model.DTO;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Utility.Helpers;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.Athena.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Fanex.Athena.Common.Services
{
    public class CustomerClassService : ICustomerClassService
    {
        private readonly IMemoryCacheService _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAppSettings _appSettings;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public CustomerClassService(
            IMemoryCacheService cache,
            IHttpContextAccessor httpContextAccessor,
            IAppSettings appSettings,
            HttpClient httpClient,
            ILogger<CustomerClassService> logger)
        {
            _appSettings = appSettings;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<string>> GetAllCustomerClassId()
        {
            var result = new List<string> { "0" };
            var listCategory = await GetCustomerClassCategories();
            result.AddRange(listCategory
                .OrderBy(_ => _.CustomerClass)
                .Select(_ => _.CustomerClass.ToString())
                );
            return result.Distinct();
        }

        public async Task<IEnumerable<RiskLevel>> GetAllRiskLevel()
        {
            var customerClassCategories = await GetCustomerClassCategories();

            customerClassCategories.Add(
                new CustomerClassCategoryModel
                {
                    CustomerClass = 0,
                    CustomerClassName = RiskLevel.Uncategorized,
                    SabaRiskLevel = RiskLevel.Uncategorized
                }
            );

            var riskLevelList = customerClassCategories.GroupBy(_ => _.SabaRiskLevel).Select((_, index) => new RiskLevel
            {
                Id = index.ToString(),
                Key = _.Key,
                Name = ViewResources.ResourceManager.GetString(_.Key) ?? _.Key,
                CustomerClassList = _.OrderBy(cc => cc.CustomerClass).Select(cc => cc.CustomerClass.ToString()).Distinct()
            }).OrderBy(_ => _, new RiskLevelComparer());

            return riskLevelList;
        }

        private async Task<List<CustomerClassCategoryModel>> GetCustomerClassCategories()
        {
            try
            {
                var customerClassCategory = _cache.Get<List<CustomerClassCategoryModel>>(CacheKey.MemorycacheCustomerClassCategories);

                if (customerClassCategory != null)
                {
                    return customerClassCategory;
                }

                var uri = new Uri(_appSettings.CustomerClassCategoryApiUrl);
                var apiResult = await _httpClient.GetStringAsync(uri);
                var result = JsonConvert.DeserializeObject<List<CustomerClassCategoryModel>>(apiResult);

                if (result.Any())
                {
                    _cache.Set(CacheKey.MemorycacheCustomerClassCategories, result);
                    _cache.SetWithoutExpired(CacheKey.MemorycacheCustomerClassCategories + "_backup", result);
                    return result;
                }

                customerClassCategory = _cache.Get<List<CustomerClassCategoryModel>>(CacheKey.MemorycacheCustomerClassCategories + "_backup");

                return customerClassCategory ?? new List<CustomerClassCategoryModel>();
            }
            catch (Exception exception)
            {
                _logger.LogError(LogMessageHelper.GenerateMessage(_httpContextAccessor.HttpContext, exception));
            }

            return new List<CustomerClassCategoryModel>();
        }
    }
}
