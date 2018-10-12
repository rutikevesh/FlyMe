using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlyMe.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FlyMe.Models;
using Microsoft.EntityFrameworkCore;

namespace FlyMe.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly FlyMeContext _context;

        public StatisticsController(FlyMeContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Stats()
        {
            var mostSoldFlights = _context.Ticket.Include(ticket => ticket.Flight)
                                                .ThenInclude(Flight => Flight.Airplane)
                                             .Include(ticket => ticket.Flight)
                                                .ThenInclude(Flight => Flight.SourceAirport)
                                             .Include(ticket => ticket.Flight)
                                                .ThenInclude(Flight => Flight.DestAirport)
                                                 .Where(ticket => (ticket.Buyer != null))
                                                .GroupBy(ticket => ticket.Flight)
                                                .Select(o => new SoldFlightsView
                                                {
                                                    Flight = o.Key,
                                                    TicketsSold = o.Count()
                                                })
                                                .OrderByDescending(o => o.TicketsSold)
                                                .ToList();
            var mostVisitedAirports = (from acronyms in _context.Airport.Select(airport => airport.Acronyms)
                                       select new CountryVisitorsAmount
                                       {
                                           AirportAcronims = acronyms,
                                           numberOfVisitors = 0
                                       }).ToList();

            foreach (var flight in mostSoldFlights)
            {
                foreach (var airport in mostVisitedAirports)
                {
                    if (flight.Flight.DestAirport.Acronyms == airport.AirportAcronims)
                        airport.numberOfVisitors += flight.TicketsSold;
                }
            }
            return View(mostVisitedAirports);
        }

    }
}