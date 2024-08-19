using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.UserDtos.MemberDtos
{
    public class MemberOrderGetDtoForProfile
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int NumberOfSeats { get; set; }
        public decimal TotalPrice { get; set; }
        public string MovieName { get; set; }
        public DateTime ShowDateTime { get; set; }
        public string HallName { get; set; }
        public string Language { get; set; }
        public List<int> SeatNumbers { get; set; }
    }
}
