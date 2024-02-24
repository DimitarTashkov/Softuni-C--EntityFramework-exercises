using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();
            //9 import suppliers
            //string suppliersJson = File.ReadAllText("../../../Datasets/suppliers.json");
            //Console.WriteLine(ImportSuppliers(context, suppliersJson));

            //10 import parts
            //string partsJson = File.ReadAllText("../../../Datasets/parts.json");
            //Console.WriteLine(ImportSuppliers(context, partsJson));

            //11
            string carsJson = File.ReadAllText("../../../Datasets/cars.json");
            Console.WriteLine(ImportCars(context,carsJson));
        }
        //9 import suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<CarDealerProfile>());
            IMapper mapper = new Mapper(config);
            SupplierDTO[] suppliesDto = JsonConvert.DeserializeObject<SupplierDTO[]>(inputJson);

            Supplier[] suppliers = mapper.Map<Supplier[]>(suppliesDto);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();
            return $"Successfully imported {suppliers.Length}.";
        }
        //10 Import parts
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
           var config = new MapperConfiguration(cfg => cfg.AddProfile<CarDealerProfile>());
            IMapper mapper = new Mapper(config);

            PartsDTO[] partsDto = JsonConvert.DeserializeObject<PartsDTO[]>(inputJson);
            var supplierId = context.Suppliers.Select(x => x.Id).ToArray();
            Part[] parts = mapper.Map<Part[]>(partsDto.Where(p => supplierId.Contains(p.SupplierId)));

            context.Parts.AddRange(parts);
            context.SaveChanges();
            return $"Successfully imported {parts.Length}.";
        }
        //11
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<CarDealerProfile>());
            IMapper mapper = new Mapper(config);
            CarDTO[] carDTOs = JsonConvert.DeserializeObject<CarDTO[]>(inputJson);
            List<Car> cars = new List<Car>();
            foreach (var carDto in carDTOs)
            {
                Car car = mapper.Map<Car>(carDto);

                var carPartsId = carDto.PartsId.Select(x => x.Id).ToArray();

                var carParts = new List<PartCar>();
                foreach (var carPartId in carPartsId)
                {
                    carParts.Add(new PartCar
                    {
                        Car = car
                        ,
                        PartId = carPartId
                    });
                }
                car.PartsCars = carParts;
                cars.Add(car);
            }
            context.Cars.AddRange(cars);
            context.SaveChanges();
            return $"Successfully imported {cars.Count}";
        }

    }
}