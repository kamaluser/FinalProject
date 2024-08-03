using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.Entites
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public bool NeedsPasswordReset { get; set; }
    }
}
