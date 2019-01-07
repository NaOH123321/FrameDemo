using System;
using System.Collections.Generic;
using System.Text;
using FrameDemo.Core.Entities;
using FrameDemo.Infrastructure.Database.EntityConfigurations;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace FrameDemo.Infrastructure.Database
{
    public class MyContext :DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new SampleConfiguration());
        }

        public DbSet<Sample> Samples { get; set; }
    }
}
