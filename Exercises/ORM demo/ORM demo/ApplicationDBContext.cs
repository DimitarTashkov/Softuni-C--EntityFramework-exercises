using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ORM_demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM_demo
{
    public class ApplicationDBContext : DbContext
    {
        private const string connectionString = "Server=DESKTOP-II4I5AG\\SQLEXPRESS;Database=MinionsDB;Integrated Security=true;TrustServerCertificate=True";


        public DbSet<Town> Towns { get; set; }
        public DbSet<Country> Countries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
