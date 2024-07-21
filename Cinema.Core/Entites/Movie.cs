using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.Entites
{
    public class Movie:AuditEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string TrailerLink { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string AgeLimit { get; set; }
        public string Photo { get; set; }

        public List<Session> Sessions { get; set; }
        public List<MovieLanguage> MovieLanguages { get; set; }
    }
}
