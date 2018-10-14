using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlyMe.Models
{
    public class Airplane
    {
        [Key]
        public int Id { get; set; }

        public int Capacity { get; set; }

        public string Model { get; set; }
    }
}