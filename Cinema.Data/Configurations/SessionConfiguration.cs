using Cinema.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
                   .HasForeignKey(s => s.MovieId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Hall)
                   .WithMany(h => h.Sessions)
                   .HasForeignKey(s => s.HallId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Language)
                   .WithMany()
                   .HasForeignKey(s => s.LanguageId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(s => s.Orders)
                   .WithOne(o => o.Session)
                   .HasForeignKey(o => o.SessionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
