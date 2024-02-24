namespace Medicines.DataProcessor
{
    using Boardgames.Helpers;
    using Medicines.Data;
    using Medicines.Data.Models;
    using Medicines.Data.Models.Enums;
    using Medicines.DataProcessor.ExportDtos;
    using Newtonsoft.Json;
    using System.Globalization;
    using System.Xml.Linq;

    public class Serializer
    {
        public static string ExportPatientsWithTheirMedicines(MedicinesContext context, string date)
        {

            DateTime dateParsed;
            bool isProductionDateValid = DateTime.TryParseExact(date,
                "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dateParsed);
            var patients = context.Patients.Where(p => p.PatientsMedicines.Any(pm => pm.Medicine.ProductionDate > dateParsed))
                .ToArray()
                .Select(p => new ExportPatientsDTO
                {
                    Name = p.FullName
                    ,
                    AgeGroup = p.AgeGroup.ToString().ToLower()
                    ,
                    Gender = p.Gender.ToString().ToLower()
                    ,
                    Medicines = p.PatientsMedicines.Where(pm => pm.Medicine.ProductionDate > dateParsed)
                    .Select(pm => new ExportMedicinesDTO()
                    {
                        Category = pm.Medicine.Category.ToString().ToLower()
                        ,
                        Name = pm.Medicine.Name
                        ,
                        Price = pm.Medicine.Price.ToString("f2")
                        ,
                        Producer = pm.Medicine.Producer
                        ,
                        BestBefore = pm.Medicine.ExpiryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                    }).OrderByDescending(pm => pm.BestBefore)
                    .ThenBy(pm => pm.Price)
                    .ToArray()

                }).OrderByDescending(p => p.Medicines.Count())
                .ThenBy(p => p.Name)
                .ToArray();
            return XmlSerializationHelper.Serialize<ExportPatientsDTO[]>(patients, "Patients");

        }

        public static string ExportMedicinesFromDesiredCategoryInNonStopPharmacies(MedicinesContext context, int medicineCategory)
        {
            var medicines = context.Medicines.Where(m => m.Category == (Category)medicineCategory && m.Pharmacy.IsNonStop == true)
                .ToArray()
                .Select(m => new
                {
                    Name = m.Name
                    ,
                    Price = m.Price.ToString("f2")
                    ,
                    Pharmacy = new
                    {
                        Name = m.Pharmacy.Name
                        ,
                        PhoneNumber = m.Pharmacy.PhoneNumber
                    }
                })
                .OrderBy(m => m.Price)
                .ThenBy(m => m.Name
                .ToArray());

            return JsonConvert.SerializeObject(medicines,Formatting.Indented);
        }
    }
}
