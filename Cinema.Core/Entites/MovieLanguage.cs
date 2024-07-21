using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.Entites
{
    public class MovieLanguage
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public int LanguageId { get; set; }
        public Language Language { get; set; }
    }
}
