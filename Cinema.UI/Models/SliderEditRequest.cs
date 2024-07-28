﻿using System.Text.Json.Serialization;

namespace Cinema.UI.Models
{
    public class SliderEditRequest
    {
        public int? Order { get; set; }
        public IFormFile? Image { get; set; }
        [JsonIgnore]
        public string? ImageUrl { get; set; }    
    }
}
