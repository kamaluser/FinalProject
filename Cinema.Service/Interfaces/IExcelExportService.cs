using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Interfaces
{
    public interface IExcelExportService
    {
        Task<byte[]> ExportOrdersAsync();
        Task<byte[]> ExportSessionsAsync();
    }
}
