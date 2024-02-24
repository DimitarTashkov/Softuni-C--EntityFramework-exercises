using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM_demo.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public ICollection<Town>? Towns { get; set; }
        
    }
}
