using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ImportDtos
{
    [XmlType("Pharmacy")]
    public class ImportPharmaciesDTO
    {
        [Required]
        [XmlAttribute("non-stop")]
        public string IsNonStop { get; set; }
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        [XmlElement("Name")]
        public string Name { get; set; }
        [Required]
        [MaxLength(14)]
        [RegularExpression("\\(\\d{3}\\) \\d{3}-\\d{4}")]
        [XmlElement("PhoneNumber")]
        public string PhoneNumber { get; set; }
        [XmlArray("Medicines")]
        public ImportMedicinesDTO[] Medicines { get; set; }
    }
}
