using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Castle.Core.Resource;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();

            //9
            //string inputSupplierXml = File.ReadAllText("../../../Datasets/suppliers.xml");
            //Console.WriteLine(ImportSuppliers(context,inputSupplierXml));

            //10
            //string inputPartsXml = File.ReadAllText("../../../Datasets/parts.xml");
            //Console.WriteLine(ImportParts(context,inputPartsXml));

            //11
            //string inputCarsXml = File.ReadAllText("../../../Datasets/cars.xml");
            //Console.WriteLine(ImportCars(context,inputCarsXml));
            //12
            //string inputCustomersXml = File.ReadAllText("../../../Datasets/customers.xml");
            //Console.WriteLine(ImportCustomers(context,inputCustomersXml));
            //13
            //string inputSalesXml = File.ReadAllText("../../../Datasets/sales.xml");
            //Console.WriteLine(ImportSales(context, inputSalesXml));

            //14
            //Console.WriteLine(GetCarsWithDistance(context));

            //15
            //Console.WriteLine(GetCarsFromMakeBmw(context));

            //16
            //Console.WriteLine(GetLocalSuppliers(context));

            //17
            //Console.WriteLine(GetCarsWithTheirListOfParts(context));

            //18
            Console.WriteLine(GetTotalSalesByCustomer(context));


        }

        public static Mapper GetMapper() 
        {
            var cfg = new MapperConfiguration(c => c.AddProfile<CarDealerProfile>());
            return new Mapper(cfg);
        }
        //9
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            //1. Create a xml serialiser
            XmlSerializer xmlSerialiser = new XmlSerializer(typeof(ImportSupplierDTO[]),
                new XmlRootAttribute("Suppliers"));

            //2. Deserialise things
            using var reader = new StringReader(inputXml);
            ImportSupplierDTO[] importSupplierDTOs = (ImportSupplierDTO[]) xmlSerialiser.Deserialize(reader);

            //3. Create mapper
            var mapper = GetMapper();
            Supplier[] suppliers = mapper.Map<Supplier[]>(importSupplierDTOs);

            //4. Commit changese
            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}";

        }
        //10
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerialiser = new XmlSerializer(typeof(ImportPartsDTO[])
                ,new XmlRootAttribute("Parts"));

            using StringReader reader = new StringReader(inputXml);

            var mapper = GetMapper();

            ImportPartsDTO[] partsDTOs = (ImportPartsDTO[])xmlSerialiser.Deserialize(reader);
            var supplierIds = context.Suppliers.Select(x => x.Id).ToArray();
            Part[] parts = mapper.Map<Part[]>(partsDTOs.Where(x => supplierIds.Contains(x.SupplierId)));

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Length}";
        }
        //11
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlSerializer serialiser = new XmlSerializer(typeof(ImportCarsDTO[])
                , new XmlRootAttribute("Cars"));
            using StringReader reader = new StringReader(inputXml);
            var mapper = GetMapper();
            ImportCarsDTO[] carsDTO = (ImportCarsDTO[])serialiser.Deserialize(reader);
            List<Car> cars = new List<Car>();
            foreach (var carDTO in carsDTO)
            {
                Car car = mapper.Map<Car>(carDTO);

                int[] carPartIds = carDTO.PartsIds.Select(x => x.Id)
                    .Distinct()
                    .ToArray();

                var carParts = new List<PartCar>();

                foreach (var carPartId in carPartIds)
                {
                    carParts.Add(new PartCar
                    {
                        Car = car
                        ,
                        PartId = carPartId
                    }) ;
                }
                car.PartsCars = carParts;
                cars.Add(car);
            }
            context.Cars.AddRange(cars);
            context.SaveChanges();
            return $"Successfully imported {cars.Count}";
        }
        //12
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImportCustomersDTO[]),
                new XmlRootAttribute("Customers"));

            using StringReader reader = new StringReader(inputXml);

            var mapper = GetMapper();

            ImportCustomersDTO[] customersDTOs = (ImportCustomersDTO[])serializer.Deserialize(reader);
            Customer[] customers = mapper.Map<Customer[]>(customersDTOs);
            context.Customers.AddRange(customers);
            context.SaveChanges();
            return $"Successfully imported {customers.Length}";

        }
        //13
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlSerializer serialiser = new XmlSerializer(typeof(ImportsSalesDTO[])
                , new XmlRootAttribute("Sales"));

            using StringReader reader = new StringReader(inputXml);
            var mapper = GetMapper();

            int[] carIds = context.Cars.Select(c => c.Id).ToArray();

            ImportsSalesDTO[] importsSalesDTOs = (ImportsSalesDTO[])serialiser.Deserialize(reader);
            Sale[] sales = mapper.Map<Sale[]>(importsSalesDTOs.Where(s => carIds.Contains(s.CarId)));

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}";

        }
        //14
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var mapper = GetMapper();
            var carsWithDistance = context.Cars.Where(c => c.TraveledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ProjectTo<ExportCarsWithDistance>(mapper.ConfigurationProvider)
                .ToArray();
            XmlSerializer serializer = new XmlSerializer(typeof(ExportCarsWithDistance[])
                , new XmlRootAttribute("cars"));
            var xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);
            StringBuilder sb = new StringBuilder();

            using StringWriter sw = new StringWriter(sb);
            {
                serializer.Serialize(sw, carsWithDistance, xsn);
            }
            return sb.ToString().TrimEnd();

        }
        //15
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var mapper = GetMapper();
            var cars = context.Cars
                .Where(c => c.Make == "BMW")
                .Select(c => new ExportCarsWithDistanceDTO
                {
                    Id = c.Id
                    ,Model = c.Model
                    ,TraveledDistance = c.TraveledDistance
                })
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(ExportCarsWithDistanceDTO[])
                , new XmlRootAttribute("cars"));

            var xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);
            StringBuilder sb = new StringBuilder();

            using StringWriter sw = new StringWriter(sb);
            {
                serializer.Serialize(sw, cars, xsn);
            }
            return sb.ToString().TrimEnd();
        }

        //16
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var mapper = GetMapper();
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new GetLocalSuppliersDTO
                {
                    Id = s.Id
                    ,
                    Name = s.Name
                    ,
                    Count = s.Parts.Count
                })
                .ToArray();
            XmlSerializer serializer = new XmlSerializer(typeof(GetLocalSuppliersDTO[])
                , new XmlRootAttribute("suppliers"));

            var xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);
            StringBuilder sb = new StringBuilder();

            using StringWriter sw = new StringWriter(sb);
            {
                serializer.Serialize(sw, suppliers, xsn);
            }
            return sb.ToString().TrimEnd();


        }
        //17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var mapper = GetMapper();
            var cars = context.Cars.Select(c => new ExportCarsWithPartsDTO
            {
                Make = c.Make
                ,
                Model = c.Model
                ,
                TraveledDistance = c.TraveledDistance
                ,
                Parts = c.PartsCars.Select(cp => new ExportCarsPartsDTo
                {
                    Name = cp.Part.Name
                    ,
                    Price = cp.Part.Price
                })
                .OrderByDescending(cp => cp.Price)
                .ToArray()
            }).OrderByDescending(c => c.TraveledDistance)
            .ThenBy(c => c.Model)
            .Take(5)
            .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(ExportCarsWithPartsDTO[])
                , new XmlRootAttribute("cars"));

            var xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);
            StringBuilder sb = new StringBuilder();

            using StringWriter sw = new StringWriter(sb);
            {
                serializer.Serialize(sw, cars, xsn);
            }
            return sb.ToString().TrimEnd();

        }

        //18
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var totalSales = context.Customers
               .Where(c => c.Sales.Any())
               .Select(c => new ExportSalesPerCustomerDTO
               {
                   FullName = c.Name,
                   BoughtCars = c.Sales.Count,
                   SpentMoney = c.Sales.Sum(s =>
                       s.Car.PartsCars.Sum(pc =>
                           Math.Round(c.IsYoungDriver ? pc.Part.Price * 0.95m : pc.Part.Price, 2)
                       )
                   )
               })
               .OrderByDescending(s => s.SpentMoney)
               .ToArray();
            XmlSerializer serializer = new XmlSerializer(typeof(ExportSalesPerCustomerDTO[])
               , new XmlRootAttribute("customers"));

            var xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);
            StringBuilder sb = new StringBuilder();

            using StringWriter sw = new StringWriter(sb);
            {
                serializer.Serialize(sw, totalSales, xsn);
            }
            return sb.ToString().TrimEnd();

           

        }
    }
    
}