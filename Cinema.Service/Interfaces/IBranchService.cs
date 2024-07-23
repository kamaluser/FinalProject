using Cinema.Service.Dtos;
using Cinema.Service.Dtos.BranchDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Interfaces
{
    public interface IBranchService
    {
        int Create(AdminBranchCreateDto createDto);
        PaginatedList<AdminBranchGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        void Edit(int id, AdminBranchEditDto editDto);
        void Delete(int id);
        AdminBranchGetDto GetById(int id);
        List<AdminBranchGetDto> GetAll(string? search = null);
    }
}
