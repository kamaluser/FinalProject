using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.UserDtos.MemberDtos
{
    public class MemberProfileGetDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public bool HasPassword { get; set; }
        public bool IsGoogleLogin { get; set; }

        public List<MemberOrderGetDtoForProfile> Orders { get; set; }
    }
}
