using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Persistence
{
    public class DataContext : DbContext
    {
        public DbSet<Image> Images { get; set; }
        public object Users { get; internal set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("FileName=image.db");
        }
    }
}