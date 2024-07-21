using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.Entites
{
    public class Seat:BaseEntity
    {
        public int SeatId { get; set; }
        public int Number { get; set; } 

        public int HallId { get; set; }
        public Hall Hall { get; set; }

        public ICollection<OrderSeat> OrderSeats { get; set; }
    }
}
