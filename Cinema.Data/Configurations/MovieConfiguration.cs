﻿using Cinema.Core.Entites;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Data.Configurations
{
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Title).IsRequired().HasMaxLength(200);
            builder.Property(m => m.Description).IsRequired();
            builder.Property(m => m.TrailerLink).IsRequired().HasMaxLength(500);
            builder.Property(m => m.ReleaseDate).IsRequired();
            builder.Property(m => m.AgeLimit).IsRequired();
            builder.Property(m => m.Photo).IsRequired().HasMaxLength(500);

            builder.HasMany(m => m.Sessions)
                   .WithOne(s => s.Movie)
                   .HasForeignKey(s => s.MovieId);

            builder.HasMany(m => m.MovieLanguages)
                   .WithOne(ml => ml.Movie)
                   .HasForeignKey(ml => ml.MovieId);
        }
    }
}
