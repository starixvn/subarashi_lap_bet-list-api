using BetListNPOI.SS.UserModel;
using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.BetList.Core.Builder;
using Fanex.BetList.Core.Entities;

namespace Fanex.Athena.BetList.Excel.Tests.TestClasses
{
    public class BetListExcelTestClass : BetListExcel
    {
        public BetListExcelTestClass(BetListShowingOption showingOption, int licenseeId, IEnumerable<int> hiddenColumnIndexes, IAppSettings appSettings, IIPService ipService, ITicketHelper ticketHelper) 
            : base(showingOption, licenseeId, hiddenColumnIndexes, appSettings, ipService, ticketHelper)
        { 
        }

        public void ProtectedBuild(IBetListExcelBuilder builder, TicketResult ticketResult, string title)    
            => base.Build(builder, ticketResult, title);

        public void ProtectedAssignSheetsData(IBetListExcelBuilder builder, TicketResult ticketResult, string title, int startIndex)
            => base.AssignSheetsData(builder, ticketResult, title, startIndex);

        public void ProtectedBuildTicket(IBetListExcelBuilder builder, TicketResult ticketResult, LrfTicket ticket)
            => base.BuildTicket(builder, ticketResult, ticket);

        public void ProtectedInitializeFonts(IBetListExcelBuilder builder)
            => base.InitializeFonts(builder);

        public void ProtectedProcessUi(IWorkbook workbook)
            => base.ProcessUi(workbook);

        public void ProtectedFixBugIOSMobile(IWorkbook workbook)
            => base.FixBugIOSMobile(workbook);
    }
}
