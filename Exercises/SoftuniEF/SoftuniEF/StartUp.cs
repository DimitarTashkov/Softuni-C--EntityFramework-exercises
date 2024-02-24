using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();
            Console.WriteLine(GetEmployeesByFirstNameStartingWithSa(context));
        }
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName
                    , e.LastName
                    , e.MiddleName
                    , e.JobTitle
                    , e.Salary
                }).ToList();
            string result = string.Join(Environment.NewLine, employees.Select(e => $"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2} "));
            return result;
        }
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees.Select(e => new
            {
                e.FirstName
                , e.Salary
            }
            ).Where(s => s.Salary > 50000)
            .OrderBy(e => e.FirstName)
            .ToList();
            return string.Join(Environment.NewLine, employees.Select(e => $"{e.FirstName} - {e.Salary:f2}"));
        }
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var rndEmployess = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .Select(e => new
                {
                    e.FirstName
                    ,
                    e.LastName
                    ,
                    e.Department.Name
                    ,
                    e.Salary
                })
                .OrderBy(e => e.Salary).ThenBy(e => e.FirstName);
            return string.Join(Environment.NewLine, rndEmployess.Select(rnd => $"{rnd.FirstName} {rnd.LastName} from {rnd.Name} - ${rnd.Salary:f2}"));
        }
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address address = new Address()
            {
                AddressText = "Vitoshka 15"
                , TownId = 4
            };
            var employee = context.Employees.FirstOrDefault(x => x.LastName == "Nakov");

            employee.Address = address;

            context.SaveChanges();

            var employees = context.Employees.Select(e => new
            {
                e.AddressId
                ,
                e.Address.AddressText
            }).OrderByDescending(e => e.AddressId)
               .Take(10).ToList();


            return string.Join(Environment.NewLine, employees.Select(e => $"{e.AddressText}"));

        }
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName
                    ,
                    e.LastName
                    ,
                    e.JobTitle
                    ,
                    e.Salary
                }).Where(e => e.FirstName.StartsWith("Sa"))
                .OrderBy(e => e.FirstName )
                .ThenBy(e => e.LastName)
                .ToList();

            return string.Join(Environment.NewLine, employees.Select(e => $"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})"));
        }
    }
}
