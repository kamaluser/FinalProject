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
    public class OrderSeatConfiguration : IEntityTypeConfiguration<OrderSeat>
    {
        public void Configure(EntityTypeBuilder<OrderSeat> builder)
        {
            builder.HasKey(os => new { os.OrderId, os.SeatId });

            builder.HasOne(os => os.Order)
                   .WithMany(o => o.OrderSeats)
                   .HasForeignKey(os => os.OrderId);

            builder.HasOne(os => os.Seat)
                   .WithMany(se => se.OrderSeats)
                   .HasForeignKey(os => os.SeatId);
        }
    }
}
