using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.OrderDtos
{
    public class OrderStatsByDateDto
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}
