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
    public class HallConfiguration : IEntityTypeConfiguration<Hall>
    {
        public void Configure(EntityTypeBuilder<Hall> builder)
        {
            builder.HasKey(h => h.Id);
            builder.Property(h => h.Name).IsRequired().HasMaxLength(100);
            builder.Property(h => h.SeatCount).IsRequired();

            builder.HasMany(h => h.Sessions)
                   .WithOne(s => s.Hall)
                   .HasForeignKey(s => s.HallId);

            builder.HasMany(h => h.Seats)
                   .WithOne(se => se.Hall)
                   .HasForeignKey(se => se.HallId);
        }
    }
}