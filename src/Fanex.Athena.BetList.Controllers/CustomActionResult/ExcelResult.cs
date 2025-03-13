namespace Fanex.Athena.BetList.Controllers.CustomActionResult
{
    using BetListNPOI.SS.UserModel;
    using Microsoft.AspNetCore.Mvc;

    public class ExcelResult : ActionResult
    {
        private readonly IWorkbook _hssfWorkBook;
        private readonly string _fileName;

        public ExcelResult(IWorkbook hssfWorkbook, string fileName = null)
        {
            _hssfWorkBook = hssfWorkbook;
            _fileName = fileName;
        }

        public override void ExecuteResult(ActionContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = "application/vnd.ms-excel";
            if (!string.IsNullOrWhiteSpace(_fileName))
            {
                response.Headers.TryAdd("Content-disposition", "attachment; filename=" + _fileName + ".xls");
            }
            _hssfWorkBook.Write(response.Body, true);
        }
    }
}