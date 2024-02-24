﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trucks.Data.Models
{
    public class Client
    {
        public Client()
        {
            ClientsTrucks = new List<ClientTruck>();
        }
        [Key]
        public int Id { get; set; }
        [MaxLength(40)]
        [Required]
        public string Name { get; set; }
        [MaxLength(40)]
        [Required]
        public string Nationality { get; set; }
        [Required]
        public string Type { get; set; }
        public ICollection<ClientTruck> ClientsTrucks { get; set; }

    }
}