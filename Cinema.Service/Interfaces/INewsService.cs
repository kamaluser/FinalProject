using Cinema.Service.Dtos.BranchDtos;
using Cinema.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinema.Service.Dtos.NewsDtos;

namespace Cinema.Service.Interfaces
{
    public interface INewsService
    {
        int Create(AdminNewsCreateDto createDto);
        PaginatedList<AdminNewsGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        void Edit(int id, AdminNewsEditDto editDto);
        void Delete(int id);
        AdminNewsGetDto GetById(int id);
        PaginatedList<UserNewsGetDto> GetAllByPageUser(int page = 1, int size = 10);
    }
}
