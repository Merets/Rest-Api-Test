using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class UserContext : DbContext
    {
        public UserContext (DbContextOptions<UserContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<UserDTO>().HasOne(u => u.Work);
        }

        public DbSet<WebApplication1.Models.UserDTO> Users { get; set; }
        public DbSet<WebApplication1.Models.WorkDTO> Works { get; set; }
    }
}
