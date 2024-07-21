using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.Entites
{
    public class News:AuditEntity
    {
        public string Title { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
    }
}
