using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Cinema.Core.Entites
{
    public class Order:BaseEntity
    {
        public int UserId { get; set; }
        //public AppUser User { get; set; }

        public int SessionId { get; set; }
        public Session Session { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public int NumberOfSeats { get; set; }
        public decimal TotalPrice { get; set; }

        public List<OrderSeat> OrderSeats { get; set; }
    }
}
