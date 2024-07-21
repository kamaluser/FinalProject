using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.Entites
{
    public class Branch:AuditEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public List<Hall> Halls { get; set; }
    }
}
