using Cinema.Data;
using Cinema.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema.Service.Implementations
{
    public class ExcelExportService : IExcelExportService
    {
        private readonly AppDbContext _context;

        public ExcelExportService(AppDbContext context)
        {
            _context = context;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<byte[]> ExportSessionsAsync()
        {
            var sessions = await _context.Sessions
                .Where(s => !s.IsDeleted)
                .Include(s => s.Movie)
                .Include(s => s.Language)
                .Include(s => s.Hall)
                .ThenInclude(h => h.Branch)
                .ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sessions");

                worksheet.Cells[1, 1].Value = "Movie Title";
                worksheet.Cells[1, 2].Value = "Hall Name";
                worksheet.Cells[1, 3].Value = "Branch Name";
                worksheet.Cells[1, 4].Value = "Language";
                worksheet.Cells[1, 5].Value = "Show Date & Time";
                worksheet.Cells[1, 6].Value = "Price";

                for (int i = 0; i < sessions.Count; i++)
                {
                    var session = sessions[i];
                    worksheet.Cells[i + 2, 1].Value = session.Movie.Title;
                    worksheet.Cells[i + 2, 2].Value = session.Hall.Name;
                    worksheet.Cells[i + 2, 3].Value = session.Hall.Branch.Name;
                    worksheet.Cells[i + 2, 4].Value = session.Language.Name;
                    worksheet.Cells[i + 2, 5].Value = session.ShowDateTime.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cells[i + 2, 6].Value = session.Price.ToString("C");
                }

                worksheet.Cells.AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
    }
}
