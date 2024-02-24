using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicines.DataProcessor.ImportDtos
{
    public class ImportPatientsDTO
    {
        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        public string FullName { get; set; }
        [Range(0,2)]
        [Required]
        public int AgeGroup { get; set; }
        [Range(0,1)]
        [Required]
        public int Gender { get; set; }
        public int[] Medicines { get; set; }
    }
}
