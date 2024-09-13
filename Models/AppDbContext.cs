using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;


namespace LeadTask2.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Lead> LeadsData { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
