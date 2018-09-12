﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public Flight Flight { get; set; }

        [Required]
        public int Price { get; set; }


        public string Seat { get; set; }

        public int LuggageWeight { get; set; }

        [Required]
        public User Buyer { get; set; }
    }
}
