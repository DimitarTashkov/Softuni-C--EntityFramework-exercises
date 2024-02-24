namespace Medicines.DataProcessor
{
    using Boardgames.Helpers;
    using Medicines.Data;
    using Medicines.Data.Models;
    using Medicines.Data.Models.Enums;
    using Medicines.DataProcessor.ImportDtos;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data!";
        private const string SuccessfullyImportedPharmacy = "Successfully imported pharmacy - {0} with {1} medicines.";
        private const string SuccessfullyImportedPatient = "Successfully imported patient - {0} with {1} medicines.";

        public static string ImportPatients(MedicinesContext context, string jsonString)
        {
            var patientsDTO = JsonConvert.DeserializeObject<ImportPatientsDTO[]>(jsonString);
            StringBuilder sb = new StringBuilder();
            List<Patient> patients = new List<Patient>();
            foreach (var patientDTO in patientsDTO)
            {
                if(!IsValid(patientDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                Patient patient = new Patient
                {
                    FullName = patientDTO.FullName
                    ,
                    AgeGroup = (AgeGroup)patientDTO.AgeGroup
                    ,
                    Gender = (Gender)patientDTO.Gender

                };
                foreach (int medicineId in patientDTO.Medicines)
                {
                    if(patient.PatientsMedicines.Any(pm => pm.MedicineId == medicineId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    PatientMedicine patientMedicine = new PatientMedicine 
                    {
                        Patient = patient
                        ,MedicineId = medicineId
                    };

                    patient.PatientsMedicines.Add(patientMedicine);
                }
                patients.Add(patient);
                sb.AppendLine(string.Format(SuccessfullyImportedPatient, patient.FullName, patient.PatientsMedicines.Count()));
            }
            context.Patients.AddRange(patients);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportPharmacies(MedicinesContext context, string xmlString)
        {
            var pharmaciesDTOs = XmlSerializationHelper
                .Deserialize<ImportPharmaciesDTO[]>(xmlString, "Pharmacies");
            StringBuilder sb = new StringBuilder();
            List<Pharmacy> pharmacies = new List<Pharmacy>();
            foreach (var pharmaciesDTO in pharmaciesDTOs)
            {
                if(!IsValid(pharmaciesDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                bool valid;
                bool tryParse = bool.TryParse(pharmaciesDTO.IsNonStop, out valid);

                if(valid == false)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                Pharmacy pharmacy = new Pharmacy
                {
                    Name = pharmaciesDTO.Name
                    ,
                    PhoneNumber = pharmaciesDTO.PhoneNumber
                    ,IsNonStop = valid
                };
                foreach (var medicine in pharmaciesDTO.Medicines)
                {
                    if(!IsValid(medicine))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                   
                    DateTime productionDate;
                    bool isProductionDateValid = DateTime.TryParseExact(medicine.ProductionDate,
                        "yyyy-MM-dd", CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out productionDate);
                    if(!isProductionDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    
                    DateTime expiryDate;
                    bool isExpiryDateValid = DateTime.TryParseExact(medicine.ExpiryDate,
                        "yyyy-MM-dd", CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out expiryDate);

                    if (!isExpiryDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    if(productionDate.Day >= expiryDate.Day)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    Medicine medicine1 = new Medicine
                    {
                        Name = medicine.Name
                        ,
                        Price = medicine.Price
                        ,
                        ProductionDate = productionDate
                        ,
                        ExpiryDate = expiryDate
                        ,
                        Producer = medicine.Producer
                        ,
                        Category = (Category)medicine.Category
                    };
                    if(pharmacy.Medicines.Any(p => p.Name == medicine1.Name && p.Producer == medicine1.Producer))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    pharmacy.Medicines.Add(medicine1);
                }
                pharmacies.Add(pharmacy);
                sb.AppendLine(string.Format(SuccessfullyImportedPharmacy, pharmacy.Name, pharmacy.Medicines.Count()));
            }
            context.Pharmacies.AddRange(pharmacies);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
