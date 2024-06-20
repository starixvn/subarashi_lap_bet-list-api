using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.BetList.Core.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.Athena.BetList.Controllers
{
    public class SystemParlayBetListController
    {
        private readonly ITicketHelper _ticketHelper;

        private readonly ISystemParlayDetailService _systemParlayDetailService;

        private readonly IWebHostEnvironment _hostingEnvironment;

        public SystemParlayBetListController(
            ITicketHelper ticketHelper,
            ISystemParlayDetailService systemParlayDetailService,
            IWebHostEnvironment webHostEnvironment)
        {
            _ticketHelper = ticketHelper;
            _systemParlayDetailService = systemParlayDetailService;
            _hostingEnvironment = webHostEnvironment;
        }

        [HttpPost]
        public async Task<string> Detail(SystemParlayDetailMetaModel metaModel)
        {
            var contentRootPath = _hostingEnvironment.ContentRootPath;
            var viewPath = Path.Combine(contentRootPath, "Views/SystemParlayDetail/Index.cshtml");

            return await _systemParlayDetailService.BuildHtmlContent(metaModel, _ticketHelper, viewPath);
        }
    }
}
