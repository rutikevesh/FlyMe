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

        public Airplane Airplane { get; set; }

        public DateTime Date { get; set; }

    }
}
