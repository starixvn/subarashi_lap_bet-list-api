namespace Fanex.Athena.TestHelper
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.Routing;
    using NSubstitute;

    public static class WebHelper
    {
        public static void SetUpFakeControllerContext(ControllerBase controller)
        {
            var response = Substitute.For<HttpResponse>();

            var httpContext = Substitute.For<HttpContext>();
            httpContext.Response.Returns(response);
            var actionContext = new ActionContext(httpContext, new RouteData(), new ControllerActionDescriptor());

            controller.ControllerContext = new ControllerContext(actionContext);
        }
    }
}
