using System.Collections.Generic;

namespace FlyMe.Models
{
    public class Airplane
    {
        public int Id { get; set; }

        public int Capacity { get; set; }

        public List<Flight> Flights { get; set; }
    }
}