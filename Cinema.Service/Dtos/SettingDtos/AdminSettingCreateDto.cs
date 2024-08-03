using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.SettingDtos
{
    public class AdminSettingCreateDto
    {
        public IFormFile Logo { get; set; }
        public string PhoneNumber { get; set; }
        public string FacebookUrl { get; set; }
        public string YoutubeUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string TelegramUrl { get; set; }
        public string ContactAddress { get; set; }
        public string ContactEmailAddress { get; set; }
        public string ContactWorkHours { get; set; }
        public string ContactMarketingDepartment { get; set; }
        public string ContactMap { get; set; }
        public string AboutTitle { get; set; }
        public string AboutDesc { get; set; }
    }
}
