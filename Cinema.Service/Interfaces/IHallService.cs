using Cinema.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinema.Service.Dtos.HallDtos;

namespace Cinema.Service.Interfaces
{
    public interface IHallService
    {
        int Create(AdminHallCreateDto createDto);
        PaginatedList<AdminHallGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        void Edit(int id, AdminHallEditDto editDto);
        void Delete(int id);
        AdminHallGetDto GetById(int id);
        List<AdminHallGetDto> GetAll(string? search = null);
    }
}