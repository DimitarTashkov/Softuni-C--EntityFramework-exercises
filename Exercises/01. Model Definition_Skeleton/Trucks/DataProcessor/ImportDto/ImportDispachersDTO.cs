using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Trucks.DataProcessor.ImportDto
{
    [XmlType("Despatcher")]
    public class ImportDispachersDTO
    {
        [XmlElement]
        [MaxLength(40)]
        [MinLength(2)]
        [Required]
        public string Name { get; set; }
        [XmlElement]
        public string Position { get; set; }
        [XmlArray("Trucks")]
        public ImportTrucksDTO[] ImportTrucksDTO { get; set; }
    }
}
