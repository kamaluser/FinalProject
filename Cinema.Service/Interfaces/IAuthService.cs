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
        Task<string> Login(UserLoginDto loginDto);

    }
}
