namespace Fanex.Athena.Common.Utility.Services
{
    using Interfaces;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewEngines;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Routing;

    public class TemplateBuilder : ITemplateBuilder
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITempDataProvider _tempDataProvider;

        public TemplateBuilder(IRazorViewEngine viewEngine, IServiceProvider serviceProvider, ITempDataProvider tempDataProvider)
        {
            _viewEngine = viewEngine;
            _serviceProvider = serviceProvider;
            _tempDataProvider = tempDataProvider;
        }

        public async Task<string> BuildContent<T>(string path, T model)
        {
            var actionContext = GetActionContext();
            var view = FindView(actionContext, path);

            using (var output = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    view,
                    new ViewDataDictionary<T>(
                        metadataProvider: new EmptyModelMetadataProvider(),
                        modelState: new ModelStateDictionary())
                    {
                        Model = model
                    },
                new TempDataDictionary(
                        actionContext.HttpContext,
                        _tempDataProvider),
                    output,
                    new HtmlHelperOptions());

                await view.RenderAsync(viewContext);

                return output.ToString();
            }
        }


        private ActionContext GetActionContext()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = _serviceProvider;
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }

        private IView FindView(ActionContext actionContext, string viewName)
        {
            var getViewResult = _viewEngine.GetView(viewName, viewPath: viewName, isMainPage: true);
            if (getViewResult.Success)
            {
                return getViewResult.View;
            }

            var findViewResult = _viewEngine.FindView(actionContext, viewName, isMainPage: true);
            if (findViewResult.Success)
            {
                return findViewResult.View;
            }

            var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
            var errorMessage = string.Join(
                Environment.NewLine,
                new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations));

            throw new InvalidOperationException(errorMessage);
        }

    }


}