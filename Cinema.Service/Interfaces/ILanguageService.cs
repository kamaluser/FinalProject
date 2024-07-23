using Cinema.Service.Dtos;
using Cinema.Service.Dtos.LanguageDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Interfaces
{
    public interface ILanguageService
    {
        int Create(AdminLanguageCreateDto createDto);
        PaginatedList<AdminLanguageGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        void Edit(int id, AdminLanguageEditDto editDto);
        void Delete(int id);
        AdminLanguageGetDto GetById(int id);
        List<AdminLanguageGetDto> GetAll(string? search = null);
    }
}
