using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FlyMe.Models
{
    public class Ticket
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Flight")]
        public int FlightID { get; set; }
        public Flight Flight { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public int LuggageWeight { get; set; }

        public User Buyer { get; set; }
        
    }
}
