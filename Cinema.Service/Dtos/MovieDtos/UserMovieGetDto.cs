using Cinema.Service.Dtos.LanguageDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.MovieDtos
{
    public class UserMovieGetDto
    {
        public string MovieName { get; set; }
        public string MoviePhoto { get; set; }
        public List<LanguageGetDto> Languages { get; set; }
        public string AgeLimit { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}
