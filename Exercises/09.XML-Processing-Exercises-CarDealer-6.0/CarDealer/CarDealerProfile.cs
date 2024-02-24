using AutoMapper;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<ImportSupplierDTO, Supplier>();
            CreateMap<Supplier, GetLocalSuppliersDTO>();
            CreateMap<ImportPartsDTO, Part>();
            CreateMap<ImportCarsDTO, Car>();
            CreateMap<Car, ExportCarsWithDistance>();
            CreateMap<Car,ExportCarsWithPartsDTO>();
            CreateMap<ExportCarsWithDistanceDTO, Car>();
            CreateMap<ImportCustomersDTO, Customer>();
            CreateMap<ImportsSalesDTO, Sale>();
        }
    }
}
