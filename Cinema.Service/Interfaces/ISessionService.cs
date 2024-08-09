using Cinema.Service.Dtos;
using Cinema.Service.Dtos.SessionDtos;
using System;
using System.Collections.Generic;

namespace Cinema.Service.Interfaces
{
    public interface ISessionService
    {
        int Create(AdminSessionCreateDto dto);
        void Edit(int id, AdminSessionEditDto dto);
        void Delete(int id);
        AdminSessionGetDto GetById(int id);
        PaginatedList<AdminSessionGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        List<AdminSessionGetDto> GetAll();
    }
}
