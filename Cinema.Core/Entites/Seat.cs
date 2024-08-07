using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.Entites
{
    public class Seat:BaseEntity
    {
        public int Number { get; set; }
        public bool IsOrdered { get; set; }
        public DateTime? BookedFrom { get; set; }
        public DateTime? BookedUntil { get; set; }
        public int HallId { get; set; }
        public Hall Hall { get; set; }

        public List<OrderSeat> OrderSeats { get; set; }
    }
}
