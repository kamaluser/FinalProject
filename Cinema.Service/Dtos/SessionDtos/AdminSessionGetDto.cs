using Cinema.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.SessionDtos
{
    public class AdminSessionGetDto
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string MovieTitle { get; set; }
        public int HallId { get; set; }
        public string HallName { get; set; }
        public int LanguageId { get; set; }
        public string LanguageName { get; set; }
        public DateTime ShowDateTime { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public string BranchName { get; set; }
    }
}
