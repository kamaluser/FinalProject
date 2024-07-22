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
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.OrderDate).IsRequired();
            builder.Property(o => o.NumberOfSeats).IsRequired();
            builder.Property(o => o.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();

            builder.HasMany(o => o.OrderSeats)
                   .WithOne(os => os.Order)
                   .HasForeignKey(os => os.OrderId);
        }
    }

}
