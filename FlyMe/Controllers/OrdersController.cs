﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FlyMe.Data;
using FlyMe.Models;
using Accord.MachineLearning;
using Accord.Math.Distances;
using Microsoft.AspNetCore.Http;

namespace FlyMe.Controllers
{
    public class OrdersController : Controller
    {
        private readonly FlyMeContext _context;

        public OrdersController(FlyMeContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            int? recommendedDestAirportId = getRecommendedDestinationIdForCurrentUser();
            ViewBag.RecommendedTicket = getRecommendedTicketByDestFlightId(recommendedDestAirportId);

            return View(await _context.Ticket.Include(ticket => ticket.Flight)
                                                .ThenInclude(Flight => Flight.Airplane)
                                             .Include(ticket => ticket.Flight)
                                                .ThenInclude(Flight => Flight.SourceAirport)
                                             .Include(ticket => ticket.Flight)
                                                .ThenInclude(Flight => Flight.DestAirport)
                                                 .Where(ticket => ticket.Buyer == null)
                                                .OrderBy(ticket => ticket.Flight.Date)
                                                .ToListAsync());
        }

        // GET: Orders/Buy/*id*
        public async Task<IActionResult> Buy(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketBought = await _context.Ticket.FirstOrDefaultAsync(m => m.Id == id);

            if (ticketBought == null)
            {
                return NotFound();
            }

            try
            {
                ticketBought.Buyer = await _context.User.FirstOrDefaultAsync(m => m.ID == 1);
                _context.Update(ticketBought);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(ticketBought.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(UserTicketsView));
        }

        // GET: Orders/MoreInfo/*id*
        public async Task<IActionResult> MoreInfo(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var selectedTicket = await _context.Ticket.Include(ticket => ticket.Flight)
                                                .ThenInclude(Flight => Flight.Airplane)
                                             .Include(ticket => ticket.Flight)
                                                .ThenInclude(Flight => Flight.SourceAirport)
                                             .Include(ticket => ticket.Flight)
                                                .ThenInclude(Flight => Flight.DestAirport)
                                             .FirstOrDefaultAsync(m => m.Id == id);

            if (selectedTicket == null)
            {
                return NotFound();
            }

            return View(selectedTicket);
        }

        private bool TicketExists(int id)
        {
            return _context.Ticket.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> Search(DateTime? date, string from, string to, int maxPrice)
        {
            if (date.HasValue)
                ViewBag.date = date.Value.ToString("yyyy-MM-dd");

            ViewBag.from = from;
            ViewBag.to = to;

            if (maxPrice > 0)
                ViewBag.maxPrice = maxPrice;

            var dayAfterDate = date?.AddDays(1);

            return View("Index",
                await _context.Ticket.Include(ticket => ticket.Flight)
                                                .ThenInclude(Flight => Flight.Airplane)
                                             .Include(ticket => ticket.Flight)
                                                .ThenInclude(Flight => Flight.SourceAirport)
                                             .Include(ticket => ticket.Flight)
                                                .ThenInclude(Flight => Flight.DestAirport)
                    .Where(ticket =>
                    (!date.HasValue || !dayAfterDate.HasValue || (date <= ticket.Flight.Date && ticket.Flight.Date < dayAfterDate)) &&
                    (string.IsNullOrEmpty(from) || ticket.Flight.SourceAirport.Country.ToLower().Contains(from.ToLower()) || ticket.Flight.SourceAirport.City.ToLower().Contains(from.ToLower())) &&
                    (string.IsNullOrEmpty(to) || ticket.Flight.DestAirport.Country.ToLower().Contains(to.ToLower()) || ticket.Flight.DestAirport.City.ToLower().Contains(to.ToLower())) &&
                    ((maxPrice <= 0) || (maxPrice >= ticket.Price)) &&
                    (ticket.Buyer == null))
                    .OrderBy(ticket => ticket.Flight.Date)
                    .ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> MostSoldFlightsView()
        {
            return View(await _context.Ticket.Include(ticket => ticket.Flight)
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
                    .ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> UserTicketsView(int userId)
        {
            return View(await _context.Ticket.Include(ticket => ticket.Flight)
                                                .ThenInclude(Flight => Flight.Airplane)
                                             .Include(ticket => ticket.Flight)
                                                .ThenInclude(Flight => Flight.SourceAirport)
                                             .Include(ticket => ticket.Flight)
                                                .ThenInclude(Flight => Flight.DestAirport)
                    .Join(_context.User,
                        ticket => ticket.Buyer.ID,
                        user => user.ID,
                        (ticket, user) => ticket)
                    .Where(ticket => (ticket.Buyer.ID == 1))
                    .OrderByDescending(ticket => ticket.Flight.Date)
                    .ToListAsync());
        }

        private int? getRecommendedDestinationIdForCurrentUser()
        {
            // Now we will create the K-Nearest Neighbors algorithm. For a given
            // instance, its nearest 3 neighbors will be used to cast a decision.
            const int k = 3;
            var knn = new KNearestNeighbors<double[]>(k, distance: new SquareEuclidean());

            var boughtTickets = _context.Ticket.Join(_context.User,
                                                        ticket => ticket.Buyer.ID,
                                                        user => user.ID,
                                                        (ticket, user) => ticket)
                                                    .Where(ticket => ticket.Buyer != null);

            var ticketAndDestinationAirportList = boughtTickets
                .Join(_context.Flight,
                    ticket => ticket.Flight.Id,
                    flight => flight.Id,
                    (ticket, flight) => new { ticket, flight })
                .Join(_context.Airport,
                    ticketAndFlight => ticketAndFlight.flight.DestAirport.ID,
                    airport => airport.ID,
                    (ticketAndFlight, destAirport) => new { ticketAndFlight.ticket, destAirport })
                .Select(ticketAndDestAirport => new { ticketAndDestAirport.ticket.Buyer, ticketAndDestAirport.destAirport });

            // Check that we have enough data (less then k will throw exption)
            if (ticketAndDestinationAirportList.Count() <= k)
            {
                return null;
            }

            LinkedList<double[]> usersAge = new LinkedList<double[]>();
            LinkedList<int> destAirportsId = new LinkedList<int>();

            foreach (var item in ticketAndDestinationAirportList)
            {
                double[] userAgeArray = new double[] { item.Buyer.Age };
                usersAge.AddLast(userAgeArray);

                destAirportsId.AddLast(item.destAirport.ID);
            }

            double[][] inputs = usersAge.Select(a => a.ToArray()).ToArray();
            int[] outputs = destAirportsId.ToArray();

            // Learning given inputs of ages of users and outputs of destination flights of them
            knn.Learn(inputs, outputs);

            // Get the current login user info
            var currentUser = _context.User.FirstOrDefault(u => u.ID == 1);

            if (currentUser != null)
            {
                double[] currentUserAge = new double[] { currentUser.Age };
                return knn.Decide(currentUserAge);
            }

            return null;
        }

        private Ticket getRecommendedTicketByDestFlightId(int? destAirportId)
        {
            if (destAirportId == null)
            {
                return null;
            }

            return _context.Ticket.Include(ticket => ticket.Flight)
                                                .ThenInclude(Flight => Flight.Airplane)
                                             .Include(ticket => ticket.Flight)
                                                .ThenInclude(Flight => Flight.SourceAirport)
                                             .Include(ticket => ticket.Flight)
                                                .ThenInclude(Flight => Flight.DestAirport)
                                            .Where(ticket => (ticket.Buyer != null))
                                            .FirstOrDefault(ticket => ticket.Flight.DestAirport.ID == destAirportId);
        }
    }
}