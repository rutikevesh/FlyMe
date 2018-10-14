using FlyMe.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FlyMe.Models
{
    public class Flight
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [ForeignKey("SourceAirport")]
        public int? SourceAirportID { get; set; }
        public Airport SourceAirport { get; set; }

        [ForeignKey("DestAirport")]
        public int? DestAirportID { get; set; }
        public Airport DestAirport { get; set; }

        [Required]
        [ForeignKey("Airplane")]
        public int AirplaneID { get; set; }
        public Airplane Airplane { get; set; }

        [Required]
        public DateTime Date { get; set; }

    }
}
