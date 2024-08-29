using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.OrderDtos
{
    public class BookSeatDto
    {
        public int SessionId { get; set; }
        public List<int> SeatIds { get; set; }
    }

}
