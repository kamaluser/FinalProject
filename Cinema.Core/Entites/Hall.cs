using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Cinema.Core.Entites
{
    public class Hall:AuditEntity
    {
        public string Name { get; set; } 
        public int SeatCount { get; set; } 
        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        public List<Seat> Seats { get; set; }
        public List<Session> Sessions { get; set; }
    }
}
