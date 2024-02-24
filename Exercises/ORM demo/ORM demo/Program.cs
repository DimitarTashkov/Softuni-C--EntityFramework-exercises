using Microsoft.EntityFrameworkCore;
using ORM_demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM_demo
{
    public class Program
    {
        public static void Main(string[]args)
        {
            ApplicationDBContext db = new ApplicationDBContext();

            Town town = new Town
            {
                Name = "New New York"
            };
            db.Towns.Add(town);
            db.SaveChanges();


            var countries = db.Countries;
            var towns = db.Towns.Include(t => t.Country);


            foreach (var t in towns)
            {
                Console.WriteLine($"{t.Name} is in {t.Country?.Name}");
            }
        }
    }
}
