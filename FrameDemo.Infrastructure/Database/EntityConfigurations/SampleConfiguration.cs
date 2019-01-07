using System;
using System.Collections.Generic;
using System.Text;
using FrameDemo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FrameDemo.Infrastructure.Database.EntityConfigurations
{
    public class SampleConfiguration : IEntityTypeConfiguration<Sample>
    {
        public void Configure(EntityTypeBuilder<Sample> builder)
        {
            builder.Property(x => x.Author).IsRequired();
            builder.Property(x => x.Title).HasMaxLength(50);
        }
    }
}
