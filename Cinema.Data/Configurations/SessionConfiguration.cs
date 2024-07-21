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
    public class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.ShowDateTime).IsRequired();
            builder.Property(s => s.Price).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(s => s.Duration).IsRequired();

            builder.HasOne(s => s.Movie)
                   .WithMany(m => m.Sessions)
                   .HasForeignKey(s => s.MovieId);

            builder.HasOne(s => s.Hall)
                   .WithMany(h => h.Sessions)
                   .HasForeignKey(s => s.HallId);

            builder.HasMany(s => s.Orders)
                   .WithOne(o => o.Session)
                   .HasForeignKey(o => o.SessionId);
        }
    }
}
