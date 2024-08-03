using Cinema.Service.Dtos;
using Cinema.Service.Dtos.UserDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Interfaces
{
    public interface IAuthService
    {
        LoginDto Login(AdminLoginDto loginDto);
        string CreateAdmin(SuperAdminCreateAdminDto createDto);
        PaginatedList<AdminPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        List<AdminGetDto> GetAll(string? search = null);
        void Update(string id, AdminEditDto updateDto);
        void Delete(string id);
        Task UpdatePasswordAsync(AdminEditDto updatePasswordDto);
    }  
}
