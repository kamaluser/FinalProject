using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CinemaApp.Controllers
{
    [ApiController]
    public class ExcelController : Controller
    {
        private readonly IExcelExportService _excelExportService;

        public ExcelController(IExcelExportService excelExportService)
        {
            _excelExportService = excelExportService;
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("api/admin/excel/SessionDownloadExcel")]
        public async Task<IActionResult> SessionDownloadExcel()
        {
            var fileContent = await _excelExportService.ExportSessionsAsync();
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Sessions.xlsx");
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("api/admin/excel/OrderDownloadExcel")]
        public async Task<IActionResult> OrderDownloadExcel()
        {
            var fileContent = await _excelExportService.ExportOrdersAsync();
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders.xlsx");
        }
    }
}
