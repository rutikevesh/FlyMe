using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlyMe.Models
{
    public class Airport
    {
        [Key]
        public int ID { get; set; }

        public string Country { get; set; }

        public int City { get; set; }

        //public double Longitude { get; set; }

        //public double Latitude { get; set; }
    }
}
