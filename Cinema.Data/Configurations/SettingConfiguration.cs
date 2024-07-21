using Cinema.Core.Entites;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Data.Configurations
{
    public class SettingConfiguration : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Logo).IsRequired().HasMaxLength(500);
            builder.Property(s => s.PhoneNumber).IsRequired().HasMaxLength(50);
            builder.Property(s => s.FacebookUrl).HasMaxLength(200);
            builder.Property(s => s.YoutubeUrl).HasMaxLength(200);
            builder.Property(s => s.InstagramUrl).HasMaxLength(200);
            builder.Property(s => s.TelegramUrl).HasMaxLength(200);
            builder.Property(s => s.ContactAddress).IsRequired().HasMaxLength(500);
            builder.Property(s => s.ContactEmailAddress).IsRequired().HasMaxLength(100);
            builder.Property(s => s.ContactWorkHours).IsRequired().HasMaxLength(200);
            builder.Property(s => s.ContactMarketingDepartment).IsRequired().HasMaxLength(200);
            builder.Property(s => s.ContactMap).HasMaxLength(500);
        }
    }

}
