using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.Entites
{
    public class Language:BaseEntity
    {
        public string Name { get; set; }

        public List<MovieLanguage> MovieLanguages { get; set; }
    }
}
