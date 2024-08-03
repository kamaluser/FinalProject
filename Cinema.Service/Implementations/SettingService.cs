using Cinema.Core.Entites;
using Cinema.Data;
using Cinema.Service.Dtos.SettingDtos;
using Cinema.Service.Exceptions;
using Cinema.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;

namespace Cinema.Service.Implementations
{
    public class SettingService : ISettingService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SettingService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public void Create(AdminSettingCreateDto createDto)
        {
            var setting = new Setting();

            if (createDto.Logo != null)
            {
                setting.Logo = SaveLogo(createDto.Logo);
            }

            setting.PhoneNumber = createDto.PhoneNumber;
            setting.FacebookUrl = createDto.FacebookUrl;
            setting.YoutubeUrl = createDto.YoutubeUrl;
            setting.InstagramUrl = createDto.InstagramUrl;
            setting.TelegramUrl = createDto.TelegramUrl;
            setting.ContactAddress = createDto.ContactAddress;
            setting.ContactEmailAddress = createDto.ContactEmailAddress;
            setting.ContactWorkHours = createDto.ContactWorkHours;
            setting.ContactMarketingDepartment = createDto.ContactMarketingDepartment;
            setting.ContactMap = createDto.ContactMap;
            setting.AboutTitle = createDto.AboutTitle;
            setting.AboutDesc = createDto.AboutDesc;

            _context.Settings.Add(setting);
            _context.SaveChanges();
        }

        public void Edit(AdminSettingEditDto editDto)
        {
            var setting = _context.Settings.FirstOrDefault();
            if (setting == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Setting data not found.");
            }

            if (editDto.Logo != null)
            {
                setting.Logo = SaveLogo(editDto.Logo);
            }

            setting.PhoneNumber = editDto.PhoneNumber;
            setting.FacebookUrl = editDto.FacebookUrl;
            setting.YoutubeUrl = editDto.YoutubeUrl;
            setting.InstagramUrl = editDto.InstagramUrl;
            setting.TelegramUrl = editDto.TelegramUrl;
            setting.ContactAddress = editDto.ContactAddress;
            setting.ContactEmailAddress = editDto.ContactEmailAddress;
            setting.ContactWorkHours = editDto.ContactWorkHours;
            setting.ContactMarketingDepartment = editDto.ContactMarketingDepartment;
            setting.ContactMap = editDto.ContactMap;
            setting.AboutTitle = editDto.AboutTitle;
            setting.AboutDesc = editDto.AboutDesc;

            _context.SaveChanges();
        }

        public AdminSettingGetDto Get()
        {
            var setting = _context.Settings.FirstOrDefault();
            if (setting == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Setting data not found.");
            }

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

            return new AdminSettingGetDto
            {
                Logo = setting.Logo != null ? $"{baseUrl}//uploads/settings/{Path.GetFileName(setting.Logo)}" : null,
                PhoneNumber = setting.PhoneNumber,
                FacebookUrl = setting.FacebookUrl,
                YoutubeUrl = setting.YoutubeUrl,
                InstagramUrl = setting.InstagramUrl,
                TelegramUrl = setting.TelegramUrl,
                ContactAddress = setting.ContactAddress,
                ContactEmailAddress = setting.ContactEmailAddress,
                ContactWorkHours = setting.ContactWorkHours,
                ContactMarketingDepartment = setting.ContactMarketingDepartment,
                ContactMap = setting.ContactMap,
                AboutTitle = setting.AboutTitle,
                AboutDesc = setting.AboutDesc
            };
        }

        private string SaveLogo(IFormFile logo)
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/settings");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(logo.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                logo.CopyTo(fileStream);
            }

            return uniqueFileName;
        }
    }
}
