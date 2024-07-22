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

            builder.HasOne(os => os.Order)
                   .WithMany(o => o.OrderSeats)
                   .HasForeignKey(os => os.OrderId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(os => os.Seat)
                   .WithMany(s => s.OrderSeats)
                   .HasForeignKey(os => os.SeatId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
