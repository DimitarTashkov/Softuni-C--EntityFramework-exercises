using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM_demo.Models
{
    public class Town
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        [ForeignKey(nameof(Country))]
        public int? CountryCode { get; set; }
        public Country? Country { get; set; }
    }
}
