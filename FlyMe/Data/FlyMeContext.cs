using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FlyMe.Models;

namespace FlyMe.Data
{
    public class FlyMeContext : DbContext
    {
        public FlyMeContext (DbContextOptions<FlyMeContext> options)
            : base(options)
        {
        }

        public DbSet<FlyMe.Models.User> User { get; set; }

        public DbSet<FlyMe.Models.Airplane> Airplane { get; set; }

        public DbSet<FlyMe.Models.Airport> Airport { get; set; }

        public DbSet<FlyMe.Models.Flight> Flight { get; set; }

        public DbSet<FlyMe.Models.Ticket> Ticket { get; set; }
    }
}
