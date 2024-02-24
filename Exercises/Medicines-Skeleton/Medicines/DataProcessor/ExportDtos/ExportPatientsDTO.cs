using Medicines.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ExportDtos
{
    [XmlType("Patient")]
    public class ExportPatientsDTO
    {
        [XmlAttribute("Gender")]
        public string Gender { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("AgeGroup")]
        public string AgeGroup { get; set; }
        [XmlArray("Medicines")]
        public ExportMedicinesDTO[] Medicines { get; set; }
    }
}
