using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.P01_StudentSystem.Data.Models;
using P01_StudentSystem.P01_StudentSystem.Data.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P01_StudentSystem.P01_StudentSystem.Data
{
    public class StudentSystemContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Resource> Resources { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ResourceConfiguration());
            //not needed name interfered by convention
            modelBuilder.Entity<Course>()
                .HasKey(c => c.CourseId);
        }
    }
}
