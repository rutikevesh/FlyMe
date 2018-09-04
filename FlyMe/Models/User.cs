using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


using System.ComponentModel.DataAnnotations.Schema;

namespace FlyMe.Models
{
    public class User 
    {


        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
