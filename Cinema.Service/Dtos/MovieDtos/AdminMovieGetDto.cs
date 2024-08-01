using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.MovieDtos
{
    public class AdminMovieGetDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TrailerLink { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string AgeLimit { get; set; }
        public string Photo { get; set; }
        public List<int> LanguageIds { get; set; }
    }
}
