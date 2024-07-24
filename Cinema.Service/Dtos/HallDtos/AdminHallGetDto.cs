using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.HallDtos
{
    public class AdminHallGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SeatCount { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
    }
}