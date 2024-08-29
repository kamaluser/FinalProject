using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.SessionDtos
{
    public class UserSessionDetailsDto
    {
        public int Id { get; set; }
        public string MovieName { get; set; }
        public DateTime ShowDateTime { get; set; }
        public string HallName { get; set; }
        public string BranchName { get; set; }
        public string LanguageName { get; set; }
        public string LanguagePhoto { get; set; }
        public decimal Price { get; set; }
        public string TrailerLink { get; set; }
    }
}
