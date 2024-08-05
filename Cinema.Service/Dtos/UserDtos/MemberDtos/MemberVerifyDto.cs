using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.UserDtos.MemberDtos
{
    public class MemberVerifyDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
