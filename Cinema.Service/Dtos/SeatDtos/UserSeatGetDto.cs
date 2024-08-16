using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.SeatDtos
{
    public class UserSeatGetDto
    {
        public int SeatId { get; set; }
        public int SeatNumber { get; set; }
        public bool IsOrdered { get; set; }
    }

}
