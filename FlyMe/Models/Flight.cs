using FlyMe.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlyMe.Models
{
    public class Flight
    {
        [Required]
        [Key]
        public int Id { get; set; }

        public Airport DestAirport { get; set; }

        public Airport SourceAirport { get; set; }

        [Required]
        public Airplane Airplane { get; set; }

        [Required]
        public DateTime Date { get; set; }

    }
}
