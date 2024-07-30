﻿using Cinema.Core.Entites;

namespace Cinema.UI.Models
{
    public class MovieGetResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string TrailerLink { get; set; }

        public string AgeLimit { get; set; }
        public string Photo { get; set; }
        public List<Language>? Languages { get; set; }
    }
}