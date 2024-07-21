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
    public class SeatConfiguration : IEntityTypeConfiguration<Seat>
    {
        public void Configure(EntityTypeBuilder<Seat> builder)
        {
            builder.HasKey(se => se.Id);
            builder.Property(se => se.Number).IsRequired();

            builder.HasOne(se => se.Hall)
                   .WithMany(h => h.Seats)
                   .HasForeignKey(se => se.HallId);
        }
    }

}
