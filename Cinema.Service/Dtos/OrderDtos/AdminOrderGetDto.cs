using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.OrderDtos
{
    public class AdminOrderGetDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string EmailOfUser { get; set; }
        public string BranchName { get; set; }
        public string HallName { get; set; }
        public string MovieName { get; set; }
        public string Language { get; set; }
        public DateTime SessionDate { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<int> SeatNumbers { get; set; }
    }
}
