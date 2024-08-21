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
        [HttpGet("api/admin/excel/DownloadExcel")]
        public async Task<IActionResult> DownloadExcel()
        {
            var fileContent = await _excelExportService.ExportSessionsAsync();
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Sessions.xlsx");
        }
    }
}
