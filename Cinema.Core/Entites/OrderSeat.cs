using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.Entites
{
    public class OrderSeat
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int SeatId { get; set; }
        public Seat Seat { get; set; }
    }
}
