using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.Entites
{
    public class Slider:AuditEntity
    {
        public int Order { get; set; }
        public string Image { get; set; }
    }
}
