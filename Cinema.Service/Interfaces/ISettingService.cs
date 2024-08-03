using Cinema.Service.Dtos.LanguageDtos;
using Cinema.Service.Dtos.SettingDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Interfaces
{
    public interface ISettingService
    {
        void Edit(AdminSettingEditDto editDto);
        AdminSettingGetDto Get();
        void Create(AdminSettingCreateDto createDto);

    }
}
