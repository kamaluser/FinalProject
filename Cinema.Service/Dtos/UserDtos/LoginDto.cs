using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.UserDtos
{
    public class LoginDto
    {
        public string? Token { get; set; }
        public bool NeedsPasswordReset { get; set; }
    }
}
