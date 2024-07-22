using Cinema.Service.Dtos;
using Cinema.Service.Dtos.SliderDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Interfaces
{
    public interface ISliderService
    {
        int Create(AdminSliderCreateDto createDto);
        void Edit(int id, AdminSliderEditDto editDto);
        void Delete(int id);
        AdminSliderGetDto GetById(int id);
        PaginatedList<AdminSliderGetDto> GetAllByPage(int page = 1, int size = 10);
    }
}
