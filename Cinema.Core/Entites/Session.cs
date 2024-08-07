using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.Entites
{
    public class Session : AuditEntity
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public int HallId { get; set; }
        public Hall Hall { get; set; }

        public int LanguageId { get; set; }
        public Language Language { get; set; }

        public DateTime ShowDateTime { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }

        public List<Order> Orders { get; set; }
    }

}
