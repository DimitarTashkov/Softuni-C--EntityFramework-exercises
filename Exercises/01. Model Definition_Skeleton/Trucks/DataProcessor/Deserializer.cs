namespace Trucks.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Xml.Serialization;
    using AutoMapper;
    using Data;
    using Trucks.Data.Models;
    using Trucks.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedDespatcher
            = "Successfully imported despatcher - {0} with {1} trucks.";

        private const string SuccessfullyImportedClient
            = "Successfully imported client - {0} with {1} trucks.";

        public static string ImportDespatcher(TrucksContext context, string xmlString)
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<TrucksProfile>());
            IMapper mapper = new Mapper(config);

            XmlSerializer serializer = new XmlSerializer(typeof(ImportDispachersDTO[])
                ,new XmlRootAttribute("Despatchers"));
            StringReader reader = new StringReader(xmlString);
            ImportDispachersDTO[] importDispachersDTOs = (ImportDispachersDTO[])serializer.Deserialize(reader);
            List<Despatcher> despatchers = new List<Despatcher>();

            StringBuilder sb = new StringBuilder();
            foreach (var importDispachersDTO in importDispachersDTOs)
            {
                if(!IsValid(importDispachersDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                Despatcher despatcher = mapper.Map<Despatcher>(importDispachersDTO);
                despatchers.Add(despatcher);
                sb.AppendLine(string.Format(SuccessfullyImportedDespatcher, despatcher.Name, despatcher.Trucks.Count));
                
            }
            context.Despatchers.AddRange(despatchers);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }
        public static string ImportClient(TrucksContext context, string jsonString)
        {
            throw new NotImplementedException();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}